using MiniStore.Domain.Entities;

namespace MiniStore.Application.Saas;

public record CheckoutQuoteDto(Guid SessionId, int PlanId, string PlanName, BillingCycle Cycle, decimal Subtotal, decimal DiscountAmount, decimal Total, string Currency, string? PromotionCode, DateTime ExpiresAt);
public record PendingCheckoutDto(Guid SessionId, string CompanyName, string PlanNameEnglish, string PlanNameArabic, decimal Total, string Currency, DateTime ExpiresAt);
public interface IBillingCheckoutService
{
    Task<CheckoutQuoteDto> CreateAsync(int planId, BillingCycle cycle, string? promotionCode);
    Task CompleteAsync(Guid sessionId, string providerReference);
    Task<IReadOnlyList<PendingCheckoutDto>> GetPendingAsync();
}
