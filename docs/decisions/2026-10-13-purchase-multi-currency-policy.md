# Purchase multi-currency policy

Date: 2026-10-13  
Status: Accepted

## Decision

The company currency remains the currency in General Settings. A stored exchange rate means company-currency units for one document-currency unit. Foreign-currency resolution uses the latest tenant-owned dated rate on or before the document date; company currency always resolves to one.

Confirmed Purchase Orders and posted Goods Receipts, Vendor Bills and Supplier Payments freeze transaction currency, company currency, rate, effective date and source. Supplier amounts remain in document currency while accounting, inventory valuation and reconciliation remain in company currency.

GRNI is valued at the Goods Receipt rate. A Vendor Bill clears that frozen receipt value, posts commercial price difference to Purchase Price Variance, posts the residual receipt-to-bill currency difference to exchange gain/loss, and recognizes tax/payable at the bill rate. A Supplier Payment settles payable at its proportional bill carrying value, credits cash/bank at the payment rate and posts the residual as realized exchange gain/loss. Supplier Credit Notes use proportional frozen bill company values and current AVCO inventory removal.

## Consequences

- Historical values remain reproducible even after later rate changes.
- Users must configure dated foreign rates and realized gain/loss accounts before affected posting.
- Existing documents are migrated as company-currency values at rate one because the legacy schema did not contain enough evidence to reconstruct historical foreign rates.
- Landed Costs remain company-currency documents in this slice.
