using MiniStore.Application.Dtos.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Enums;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class DiscountSettingsService
{
    private readonly IDiscountSettingsRepository _repository;

    public DiscountSettingsService(
        IDiscountSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<DiscountSettingsDto> GetAsync()
    {
        var settings = await _repository.GetAsync();

        if (settings == null)
        {
            return new DiscountSettingsDto
            {
                Enabled = true,
                AllowLineDiscount = true,
                AllowInvoiceDiscount = true,
                AllowPercentageDiscount = true,
                AllowFixedAmountDiscount = true,
                DefaultDiscountType = DiscountType.Percentage,
                MaxLineDiscountPercent = 10,
                MaxLineDiscountAmount = 100,
                MaxInvoiceDiscountPercent = 10,
                MaxInvoiceDiscountAmount = 100,
                AllowDiscountAboveLimit = false,
                DiscountOverridePermission = "Sales.Discount.Override"
            };
        }

        return new DiscountSettingsDto
        {
            Enabled = settings.Enabled,
            AllowLineDiscount = settings.AllowLineDiscount,
            AllowInvoiceDiscount = settings.AllowInvoiceDiscount,
            AllowPercentageDiscount = settings.AllowPercentageDiscount,
            AllowFixedAmountDiscount = settings.AllowFixedAmountDiscount,
            DefaultDiscountType = settings.DefaultDiscountType,
            MaxLineDiscountPercent = settings.MaxLineDiscountPercent,
            MaxLineDiscountAmount = settings.MaxLineDiscountAmount,
            MaxInvoiceDiscountPercent = settings.MaxInvoiceDiscountPercent,
            MaxInvoiceDiscountAmount = settings.MaxInvoiceDiscountAmount,
            AllowDiscountAboveLimit = settings.AllowDiscountAboveLimit,
            DiscountOverridePermission = settings.DiscountOverridePermission,
            RowVersion = settings.RowVersion
        };
    }

    public async Task UpdateAsync(DiscountSettingsDto dto)
    {
        Validate(dto);

        var settings = await _repository.GetAsync();

        if (settings == null)
        {
            settings = new DiscountSettings(
                dto.Enabled,
                dto.AllowLineDiscount,
                dto.AllowInvoiceDiscount,
                dto.AllowPercentageDiscount,
                dto.AllowFixedAmountDiscount,
                dto.DefaultDiscountType,
                dto.MaxLineDiscountPercent,
                dto.MaxLineDiscountAmount,
                dto.MaxInvoiceDiscountPercent,
                dto.MaxInvoiceDiscountAmount,
                dto.AllowDiscountAboveLimit,
                dto.DiscountOverridePermission);

            await _repository.AddAsync(settings);
            await _repository.SaveChangesAsync();

            return;
        }

        if (dto.RowVersion == null ||
            dto.RowVersion.Length == 0 ||
            !settings.RowVersion.SequenceEqual(dto.RowVersion))
        {
            throw new InvalidOperationException(
                "Discount settings were modified by another user. Please reload the page and try again.");
        }

        settings.SetEnabled(dto.Enabled);
        settings.SetAllowLineDiscount(dto.AllowLineDiscount);
        settings.SetAllowInvoiceDiscount(dto.AllowInvoiceDiscount);
        settings.SetAllowPercentageDiscount(dto.AllowPercentageDiscount);
        settings.SetAllowFixedAmountDiscount(dto.AllowFixedAmountDiscount);
        settings.SetDefaultDiscountType(dto.DefaultDiscountType);
        settings.SetMaxLineDiscountPercent(dto.MaxLineDiscountPercent);
        settings.SetMaxLineDiscountAmount(dto.MaxLineDiscountAmount);
        settings.SetMaxInvoiceDiscountPercent(dto.MaxInvoiceDiscountPercent);
        settings.SetMaxInvoiceDiscountAmount(dto.MaxInvoiceDiscountAmount);
        settings.SetAllowDiscountAboveLimit(dto.AllowDiscountAboveLimit);
        settings.SetDiscountOverridePermission(
            dto.DiscountOverridePermission);

        await _repository.SaveChangesAsync();
    }

    private static void Validate(DiscountSettingsDto dto)
    {
        if (!dto.AllowPercentageDiscount &&
            !dto.AllowFixedAmountDiscount &&
            dto.Enabled)
        {
            throw new ArgumentException(
                "At least one discount type must be enabled when discounts are enabled.");
        }

        if (dto.DefaultDiscountType == DiscountType.Percentage &&
            !dto.AllowPercentageDiscount)
        {
            throw new ArgumentException(
                "The default discount type cannot be Percentage when percentage discounts are disabled.");
        }

        if (dto.DefaultDiscountType == DiscountType.FixedAmount &&
            !dto.AllowFixedAmountDiscount)
        {
            throw new ArgumentException(
                "The default discount type cannot be Fixed Amount when fixed amount discounts are disabled.");
        }

        if (dto.MaxLineDiscountPercent < 0 ||
            dto.MaxLineDiscountPercent > 100)
        {
            throw new ArgumentException(
                "Maximum line discount percentage must be between 0 and 100.");
        }

        if (dto.MaxInvoiceDiscountPercent < 0 ||
            dto.MaxInvoiceDiscountPercent > 100)
        {
            throw new ArgumentException(
                "Maximum invoice discount percentage must be between 0 and 100.");
        }

        if (dto.MaxLineDiscountAmount < 0)
        {
            throw new ArgumentException(
                "Maximum line discount amount cannot be negative.");
        }

        if (dto.MaxInvoiceDiscountAmount < 0)
        {
            throw new ArgumentException(
                "Maximum invoice discount amount cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(
            dto.DiscountOverridePermission))
        {
            throw new ArgumentException(
                "Discount override permission is required.");
        }
    }
}