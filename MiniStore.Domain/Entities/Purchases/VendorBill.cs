namespace MiniStore.Domain.Entities;

public enum VendorBillStatus { Draft = 1, Posted = 2, Cancelled = 3 }

public sealed class VendorBill
{
    private VendorBill() { BillNumber = SupplierInvoiceNumber = NormalizedSupplierInvoiceNumber = CurrencyCode = CreatedByUserId = string.Empty; }
    public int Id { get; private set; }
    public string BillNumber { get; private set; }
    public int SupplierId { get; private set; }
    public string SupplierInvoiceNumber { get; private set; }
    public string NormalizedSupplierInvoiceNumber { get; private set; }
    public DateOnly BillDate { get; private set; }
    public string CurrencyCode { get; private set; }
    public VendorBillStatus Status { get; private set; }
    public string CreatedByUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public List<VendorBillLine> Lines { get; private set; } = [];
    public VendorBill(string billNumber,int supplierId,string supplierInvoiceNumber,DateOnly billDate,string currencyCode,string userId)
    {
        if(string.IsNullOrWhiteSpace(billNumber)||string.IsNullOrWhiteSpace(supplierInvoiceNumber)||supplierId<=0||string.IsNullOrWhiteSpace(userId))throw new ArgumentException("Vendor bill identity is required.");
        if(string.IsNullOrWhiteSpace(currencyCode)||currencyCode.Trim().Length!=3)throw new ArgumentException("Currency code must contain exactly three letters.");
        BillNumber=billNumber.Trim(); SupplierId=supplierId; SupplierInvoiceNumber=supplierInvoiceNumber.Trim(); NormalizedSupplierInvoiceNumber=Normalize(supplierInvoiceNumber); BillDate=billDate; CurrencyCode=currencyCode.Trim().ToUpperInvariant(); CreatedByUserId=userId.Trim(); CreatedAtUtc=DateTime.UtcNow; Status=VendorBillStatus.Draft;
    }
    public void AddLine(VendorBillLine line){if(Status!=VendorBillStatus.Draft)throw new InvalidOperationException("Only draft vendor bills can be changed.");Lines.Add(line);}
    public void Post(){if(Status!=VendorBillStatus.Draft||Lines.Count==0)throw new InvalidOperationException("A draft vendor bill needs at least one line before posting.");Status=VendorBillStatus.Posted;}
    private static string Normalize(string value)=>string.Concat(value.Trim().ToUpperInvariant().Where(char.IsLetterOrDigit));
}
public sealed class VendorBillLine
{
    private VendorBillLine() { }
    public int Id { get; private set; } public int VendorBillId { get; private set; } public int GoodsReceiptLineId { get; private set; } public decimal Quantity { get; private set; } public decimal UnitPrice { get; private set; } public decimal TaxPercent { get; private set; }
    public VendorBillLine(int goodsReceiptLineId,decimal quantity,decimal unitPrice,decimal taxPercent){if(goodsReceiptLineId<=0||quantity<=0||unitPrice<0||taxPercent is <0 or >100)throw new ArgumentException("Vendor bill line values are invalid.");GoodsReceiptLineId=goodsReceiptLineId;Quantity=quantity;UnitPrice=unitPrice;TaxPercent=taxPercent;}
}
