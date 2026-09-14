using System.Data;
using Microsoft.EntityFrameworkCore;
using MiniStore.Application.Saas;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using MiniStore.Infrastructure.Persistence;

namespace MiniStore.Infrastructure.Services;

public class BillingCheckoutService(AppDbContext context, ITenantContext tenantContext) : IBillingCheckoutService
{
    public async Task<IReadOnlyList<PendingCheckoutDto>> GetPendingAsync() => await context.BillingCheckoutSessions
        .Where(x => x.Status == CheckoutStatus.Pending && x.ExpiresAt > DateTime.UtcNow)
        .Join(context.Tenants, checkout => checkout.TenantId, tenant => tenant.Id, (checkout, tenant) => new { checkout, tenant })
        .Join(context.Plans, row => row.checkout.PlanId, plan => plan.Id, (row, plan) => new PendingCheckoutDto(
            row.checkout.Id, row.tenant.Name, plan.NameEnglish, plan.NameArabic,
            row.checkout.Total, row.checkout.Currency, row.checkout.ExpiresAt))
        .OrderBy(x => x.ExpiresAt)
        .ToListAsync();
    public async Task<CheckoutQuoteDto> CreateAsync(int planId, BillingCycle cycle, string? promotionCode)
    {
        if (!Enum.IsDefined(cycle))
            throw new ArgumentException("Invalid billing cycle.", nameof(cycle));

        var tenantId = tenantContext.TenantId ?? throw new InvalidOperationException("An active company is required.");
        var plan = await context.Plans.FirstOrDefaultAsync(x => x.Id == planId && x.IsActive && x.IsPublic) ?? throw new InvalidOperationException("Plan is unavailable.");
        var subtotal = cycle == BillingCycle.Annual ? plan.AnnualPrice : plan.MonthlyPrice;
        PromotionCode? promotion = null;
        if (!string.IsNullOrWhiteSpace(promotionCode))
        {
            var normalized = promotionCode.Trim().ToUpperInvariant();
            promotion = await context.PromotionCodes.FirstOrDefaultAsync(x => x.Code == normalized);
            if (promotion is null || !promotion.CanRedeem(DateTime.UtcNow, planId) || await context.PromotionRedemptions.AnyAsync(x => x.PromotionCodeId == promotion.Id && x.TenantId == tenantId))
                throw new InvalidOperationException("Promotion code is invalid, expired or already used by this company.");
        }
        var discount = decimal.Round(subtotal * (promotion?.DiscountPercentage ?? 0) / 100m, 2);
        var checkout = new BillingCheckoutSession(tenantId, planId, cycle, promotion?.Id, subtotal, discount, plan.Currency);
        await context.BillingCheckoutSessions.AddAsync(checkout); await context.SaveChangesAsync();
        return new(checkout.Id, plan.Id, plan.NameEnglish, cycle, subtotal, discount, checkout.Total, checkout.Currency, promotion?.Code, checkout.ExpiresAt);
    }

    public async Task CompleteAsync(Guid sessionId, string providerReference)
    {
        await using var tx = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var checkout = await context.BillingCheckoutSessions.FirstOrDefaultAsync(x => x.Id == sessionId) ?? throw new InvalidOperationException("Checkout not found.");
        checkout.MarkPaid(providerReference);
        var subscription = await context.TenantSubscriptions.FirstAsync(x => x.TenantId == checkout.TenantId);
        var start = DateTime.UtcNow; var end = checkout.BillingCycle == BillingCycle.Annual ? start.AddYears(1) : start.AddMonths(1);
        subscription.ChangePlan(checkout.PlanId, checkout.BillingCycle, start, end);
        if (checkout.PromotionCodeId is int promoId)
        {
            var promo = await context.PromotionCodes.FirstAsync(x => x.Id == promoId);
            promo.Redeem(DateTime.UtcNow, checkout.PlanId);
            await context.PromotionRedemptions.AddAsync(new PromotionRedemption(promo.Id, checkout.TenantId, subscription.Id, promo.DiscountPercentage));
        }
        await context.SaveChangesAsync(); await tx.CommitAsync();
    }
}
