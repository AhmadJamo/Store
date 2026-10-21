namespace MiniStore.Web.Guidance;

public sealed record ActionableErrorLink(
    string Controller,
    string Action,
    string LabelResourceKey,
    string RequiredPermission);

public static class ActionableErrorGuidance
{
    private static readonly HashSet<string> SupportedControllers = new(StringComparer.OrdinalIgnoreCase)
    {
        "Purchases", "PurchaseReturns", "SupplierPurchasing", "PurchaseRequests",
        "PurchaseApprovalRules", "PurchaseSourcing", "SupplierQuotations", "PurchaseOrders",
        "GoodsReceipts", "GoodsReceiptReturns", "VendorBills", "SupplierCreditNotes", "SupplierPayments", "SupplierCommunications",
        "PurchaseAgreements", "PurchaseTemplates", "LandedCosts", "PurchaseDocumentReversals", "PurchaseAnalytics", "SupplierIntegrations",
        "SalesPriceLists", "SalesQuotations", "SalesOrders", "SalesDeliveries", "CustomerInvoices", "CustomerPayments", "Customers"
    };

    public static ActionableErrorLink? Resolve(
        string? message,
        string? currentController,
        Func<string, string>? localize = null)
    {
        if (string.IsNullOrWhiteSpace(message) ||
            string.IsNullOrWhiteSpace(currentController) ||
            !SupportedControllers.Contains(currentController))
            return null;

        localize ??= static value => value;

        if (message.StartsWith("Configure an exchange rate for ", StringComparison.OrdinalIgnoreCase))
            return new("Settings", "Currencies", "Open currency settings", "Administration.Access");

        if (Matches(message, localize,
                "Selected pricing tax rate was not found."))
            return new("TaxRates", "Index", "Manage tax rates", "Administration.Access");

        if (Matches(message, localize,
                "Selected sales unit was not found.",
                "Selected sales unit is inactive.",
                "Selected price-rule unit was not found.",
                "Selected price-rule unit is inactive.",
                "Selected quotation sales unit was not found.",
                "Configure a managed measurement unit for the product before adding it to a quotation.",
                "Configure a managed measurement unit for the product before pricing it.",
                "Product stock unit was not found.",
                "Sales unit and product stock unit must use the same measurement dimension."))
            return new("Settings", "Units", "Manage measurement units", "Administration.Access");

        if (Matches(message, localize,
                "Selected pricing product was not found.",
                "Selected pricing product is not available for the requested sales channel.",
                "Selected price-rule product was not found.",
                "Selected price-rule product is inactive.",
                "Selected quotation product was not found."))
            return new("Products", "Index", "Manage products", "Products.Edit");

        if (Matches(message, localize,
                "Selected price-rule category was not found.",
                "Selected price-rule category is inactive."))
            return new("ProductCategories", "Index", "Manage product categories", "Products.Edit");

        if (Matches(message, localize,
                "Selected default price list was not found.",
                "Selected default price list is inactive.",
                "Selected price list was not found.",
                "Selected price list is inactive.",
                "Selected price list is not valid on the pricing date."))
            return new("SalesPriceLists", "Index", "Open sales price lists", "SalesPricing.View");

        if (Matches(message, localize,
                "Selected quotation customer was not found.",
                "Selected quotation customer is archived.",
                "Selected quotation address was not found for this customer.",
                "Selected quotation address is archived."))
            return new("Customers", "Index", "Open customers", "Customers.View");

        if (Matches(message, localize,
                "Sales quotation was not found.",
                "Only an accepted sales quotation can be converted to a sales order.",
                "The accepted sales quotation has no lines."))
            return new("SalesQuotations", "Index", "Open sales quotations", "SalesQuotations.View");

        if (Matches(message, localize,
                "Insufficient available stock.",
                "The inventory balance required for this reservation was not found.",
                "No available quantity can be reserved for the selected sales order lines."))
            return new("InventoryBalances", "Index", "Review inventory availability", "InventoryBalances.View");

        if (Matches(message, localize,
                "Selected reservation warehouse was not found."))
            return new("Warehouses", "Index", "Manage warehouses", "Warehouses.Edit");

        if (Matches(message, localize,
                "Delivery warehouse was not found.",
                "Delivery warehouse branch was not found.",
                "Assign the delivery warehouse to a branch and inventory account before posting."))
            return new("Warehouses", "Index", "Open warehouse settings", "Warehouses.Edit");

        if (Matches(message, localize,
                "Inventory reservation was not found.",
                "Only an active sales order reservation can be delivered.",
                "Delivery line no longer matches its remaining inventory reservation."))
            return new("SalesOrders", "Index", "Open sales orders", "SalesOrders.View");

        if (Matches(message, localize,
                "A sales order with posted deliveries cannot be cancelled. Use the controlled return workflow."))
            return new("SalesDeliveries", "Index", "View delivery documents", "SalesDeliveries.View");

        if (Matches(message, localize,
                "Only a confirmed non-cancelled sales order can be invoiced.",
                "A customer invoice line does not belong to this sales order.",
                "Customer invoice quantity cannot exceed the remaining eligible source quantity.",
                "Customer invoice quantity no longer matches the remaining posted delivery quantity."))
            return new("SalesOrders", "Index", "Open sales orders", "SalesOrders.View");

        if (Matches(message, localize, "Customer invoice product was not found."))
            return new("Products", "Index", "Manage products", "Products.Edit");

        if (Matches(message, localize,
                "Assign an active asset receivable account to the customer before posting the customer invoice."))
            return new("Customers", "Index", "Open customers", "Customers.Edit");

        if (Matches(message, localize,
                "Customer invoice source warehouse was not found.",
                "Assign the customer invoice source warehouse to a branch before posting."))
            return new("Warehouses", "Index", "Open warehouse settings", "Warehouses.Edit");

        if (Matches(message, localize,
                "Customer invoice source branch was not found.",
                "The customer invoice source branch must be active before posting.",
                "Configure an active sales revenue account for the customer invoice source branch."))
            return new("Branches", "Index", "Open branches", "Administration.Access");

        if (Matches(message, localize,
                "A tax rate selected on this customer invoice no longer exists.",
                "Configure an active output tax liability account before posting a customer invoice.",
                "Configure an output tax account for every taxed customer invoice line before posting."))
            return new("TaxRates", "Index", "Manage tax rates", "Administration.Access");

        if (Matches(message, localize,
                "The posting date is not covered by a fiscal period.",
                "The fiscal period for the posting date is not open."))
            return new("Settings", "FiscalPeriods", "Open fiscal periods", "Administration.Access");

        if (Matches(message, localize,
                "Payment method was not found.",
                "The selected payment method is inactive.",
                "Payment method settlement account was not found.",
                "The payment method settlement account must be an active asset account."))
            return new("PaymentMethods", "Index", "Manage payment methods", "Administration.Access");

        if (Matches(message, localize,
                "Configure accounting posting accounts before posting a foreign-currency customer payment.",
                "Configure a realized exchange gain account before posting a foreign-currency customer payment.",
                "Configure a realized exchange loss account before posting a foreign-currency customer payment."))
            return new("Settings", "Accounting", "Open accounting settings", "Administration.Access");

        if (Matches(message, localize,
                "Insufficient physical stock for the reserved delivery.",
                "Insufficient physical stock in the reserved delivery position.",
                "Insufficient physical stock in the reserved unassigned position.",
                "Insufficient non-expired tracked stock in the reserved delivery position.",
                "Insufficient selected lot or serial stock in the reserved delivery position."))
            return new("InventoryBalances", "Index", "Review inventory availability", "InventoryBalances.View");

        if (Matches(message, localize,
                "The reservation source location was not found.",
                "The reservation source location must belong to the selected warehouse.",
                "The selected source location does not allow reservations."))
            return new("WarehouseLocations", "Index", "Configure warehouse locations", "Warehouses.Edit");

        if (Matches(message, localize,
                "No active approval rule matches this purchase request."))
            return new("PurchaseApprovalRules", "Index", "Open purchase approval rules", "PurchaseApprovalRules.Manage");

        if (Matches(message, localize,
                "Configure the direct RFQ policy before using the fast path.",
                "Direct RFQ creation is disabled by company policy. Create a purchase request instead."))
            return new("Settings", "PurchaseSourcingPolicy", "Open purchase sourcing policy", "Administration.Access");

        if (Matches(message, localize,
                "The estimated RFQ amount exceeds the direct path limit. Create a purchase request for approval."))
            return new("PurchaseRequests", "Create", "Create purchase request", "PurchaseRequests.Create");

        if (Matches(message, localize,
                "Legacy direct purchase creation is retired. Create a purchase request instead."))
            return new("PurchaseRequests", "Create", "Create purchase request", "PurchaseRequests.Create");

        if (Matches(message, localize,
                "Payment method was not found.",
                "The selected payment method is inactive."))
            return new("PaymentMethods", "Index", "Manage Payment Methods", "Administration.Access");

        if (Matches(message, localize,
                "Vendor bill exceeds the configured purchase matching tolerances."))
            return new("Settings", "PurchaseMatching", "Open purchase matching settings", "Administration.Access");

        if (Matches(message, localize,
                "A tax rate selected on this purchase no longer exists.",
                "The original purchase tax rate no longer exists.",
                "The selected purchase tax rate was not found.") ||
            message.Contains("input tax account", StringComparison.OrdinalIgnoreCase))
            return new("TaxRates", "Index", "Manage tax rates", "Administration.Access");

        if (Matches(message, localize,
                "Configure the purchase return warehouse inventory account.",
                "Configure the receipt warehouse inventory account before posting a return.",
                "Configure the receipt warehouse inventory account before posting landed costs.") ||
            message.Contains("warehouse inventory account", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("inventory account to warehouse", StringComparison.OrdinalIgnoreCase))
            return new("Warehouses", "Index", "Open warehouse settings", "Warehouses.Edit");

        if (Matches(message, localize,
                "Assign a payable account to the supplier before creating a return.",
                "Assign a payable account to the supplier before posting a supplier payment.",
                "Assign a payable account to the supplier before posting a vendor bill.",
                "Assign a payable account to the supplier before posting this purchase.") ||
            message.Contains("payable account to the supplier", StringComparison.OrdinalIgnoreCase))
            return new("Suppliers", "Index", "Manage suppliers", "Suppliers.Edit");

        if (Matches(message, localize,
                "Document sequences are required.") ||
            message.StartsWith("Settings for ", StringComparison.OrdinalIgnoreCase))
            return new("Settings", "DocumentNumbers", "Open document numbering settings", "Administration.Access");

        if (Matches(message, localize,
                "Measurement unit was not found.",
                "Purchase request measurement unit was not found.",
                "Purchase unit was not found.",
                "Supplier purchase unit was not found.",
                "Product stock unit was not found.",
                "The product stock unit was not found.",
                "Product and requested unit must use managed active units.",
                "Requested unit and product stock unit must use the same measurement dimension.",
                "Purchase unit and product stock unit must use the same measurement dimension.",
                "Supplier purchase unit is not compatible with the product stock unit."))
            return new("Settings", "Units", "Manage measurement units", "Administration.Access");

        if (Matches(message, localize,
                "Product not found.",
                "Product was not found.",
                "Purchase request product was not found.",
                "Purchase order product was not found.",
                "Configure a purchase expense account for every direct-delivery product before continuing.",
                "Returned product was not found.",
                "Purchase requests require an active purchasable product.",
                "Supplier purchasing terms require an active product.",
                "The product must have a managed stock unit before supplier purchasing terms are added."))
            return new("Products", "Index", "Manage products", "Products.Edit");

        if (Matches(message, localize,
                "Supplier not found.",
                "Supplier was not found.",
                "Add an email address to the supplier before preparing email communication.",
                "Suggested supplier was not found.",
                "One or more suppliers were not found."))
            return new("Suppliers", "Index", "Manage suppliers", "Suppliers.Edit");

        if (Matches(message, localize,
                "Configure an HTTPS callback URL before queueing external delivery."))
            return new("SupplierIntegrations", "Index", "Open supplier integrations", "SupplierIntegrations.Manage");

        if (Matches(message, localize,
                "Destination warehouse not found.",
                "Destination warehouse was not found.",
                "Receipt warehouse was not found.",
                "Purchase order warehouse was not found.",
                "Warehouse was not found.",
                "Item warehouse not found.",
                "Purchase item warehouse was not found."))
            return new("Warehouses", "Index", "Manage warehouses", "Warehouses.Edit");

        if (IsAccountingConfigurationError(message, localize))
            return new("Settings", "Accounting", "Open accounting settings", "Administration.Access");

        return null;
    }

    private static bool IsAccountingConfigurationError(string message, Func<string, string> localize) =>
        Matches(message, localize,
            "Configure accounting posting accounts before posting purchases.",
            "Configure accounting posting accounts before creating a return.",
            "Configure accounting posting accounts before posting goods receipts.",
            "Configure accounting posting accounts before posting a goods receipt return.",
            "Configure accounting posting accounts before posting a vendor bill.",
            "Configure accounting posting accounts before posting a supplier credit note.",
            "Configure accounting posting accounts before posting landed costs.",
            "Configure accounting posting accounts before posting sales deliveries.",
            "Configure a cost of sales account before posting sales deliveries.",
            "Configure accounting posting accounts before posting a customer invoice.",
            "Configure a sales revenue account before posting a customer invoice.",
            "Configure an active revenue account before posting a customer invoice.",
            "Configure a goods received not invoiced account before posting goods receipts.",
            "Configure a goods received not invoiced account before posting a goods receipt return.",
            "Configure a goods received not invoiced account before posting a vendor bill.",
            "Configure a purchase price variance account before posting a return with a cost difference.",
            "Configure a purchase price variance account before posting a vendor bill with a price difference.",
            "Configure a purchase price variance account before posting a supplier credit note with a cost difference.",
            "Configure a purchase discount account before posting a discounted purchase.",
            "Configure a purchase discount account before returning a discounted purchase.",
            "Configure a cost of sales account before posting inventory cost variance.",
            "Configure a cost of sales account before posting purchase return cost variance.",
            "Configure a cost of sales account before expensing consumed landed costs.",
            "Configure a landed cost clearing account before posting landed costs.",
            "Configure accounting posting accounts before posting a foreign-currency supplier payment.",
            "Configure a realized exchange loss account before posting a foreign-currency vendor bill.",
            "Configure a realized exchange gain account before posting a foreign-currency vendor bill.",
            "Configure a realized exchange loss account before posting a foreign-currency supplier payment.",
            "Configure a realized exchange gain account before posting a foreign-currency supplier payment.");

    private static bool Matches(
        string message,
        Func<string, string> localize,
        params string[] resourceKeys) =>
        resourceKeys.Any(key =>
            string.Equals(message, key, StringComparison.Ordinal) ||
            string.Equals(message, localize(key), StringComparison.Ordinal));
}
