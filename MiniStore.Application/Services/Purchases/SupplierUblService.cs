using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using MiniStore.Domain.Entities;

namespace MiniStore.Application.Services;

public sealed record ParsedSupplierInvoiceUbl(string InvoiceNumber,DateOnly InvoiceDate,string CurrencyCode,decimal PayableAmount,string? PurchaseOrderReference);
public sealed record ParsedSupplierOrderConfirmationUbl(string ConfirmationReference,string? PurchaseOrderReference,DateOnly ConfirmedDeliveryDate,string? Note);

public sealed class SupplierUblService
{
    private static readonly XNamespace InvoiceNs="urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
    private static readonly XNamespace OrderNs="urn:oasis:names:specification:ubl:schema:xsd:Order-2";
    private static readonly XNamespace OrderResponseNs="urn:oasis:names:specification:ubl:schema:xsd:OrderResponse-2";
    private static readonly XNamespace Cbc="urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
    private static readonly XNamespace Cac="urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";

    public string CreatePurchaseOrder(PurchaseOrder order,string supplierName)
    {
        var document=new XDocument(new XDeclaration("1.0","utf-8",null),new XElement(OrderNs+"Order",
            new XAttribute(XNamespace.Xmlns+"cbc",Cbc),new XAttribute(XNamespace.Xmlns+"cac",Cac),
            new XElement(Cbc+"UBLVersionID","2.1"),new XElement(Cbc+"ID",order.OrderNumber),new XElement(Cbc+"IssueDate",order.OrderDate.ToString("yyyy-MM-dd")),
            new XElement(Cbc+"DocumentCurrencyCode",order.CurrencyCode),new XElement(Cac+"SellerSupplierParty",new XElement(Cac+"Party",new XElement(Cac+"PartyName",new XElement(Cbc+"Name",supplierName)))),
            order.Lines.Select((line,index)=>new XElement(Cac+"OrderLine",new XElement(Cac+"LineItem",new XElement(Cbc+"ID",index+1),
                new XElement(Cbc+"Quantity",new XAttribute("unitCode",line.UnitNameSnapshot),line.OrderedQuantity.ToString(CultureInfo.InvariantCulture)),
                new XElement(Cac+"Item",new XElement(Cbc+"Name",line.ProductNameSnapshot),new XElement(Cac+"SellersItemIdentification",new XElement(Cbc+"ID",line.ProductCodeSnapshot))),
                new XElement(Cac+"Price",new XElement(Cbc+"PriceAmount",new XAttribute("currencyID",order.CurrencyCode),line.UnitPrice.ToString(CultureInfo.InvariantCulture))))))));
        return document.ToString(SaveOptions.DisableFormatting);
    }

    public ParsedSupplierInvoiceUbl ParseInvoice(string xml)
    {
        if(string.IsNullOrWhiteSpace(xml)||xml.Length>2_000_000)throw new ArgumentException("UBL invoice must be between 1 byte and 2 MB.");
        using var reader=XmlReader.Create(new StringReader(xml),new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=2_000_000});
        var root=XDocument.Load(reader,LoadOptions.None).Root??throw new ArgumentException("UBL invoice XML is empty.");
        if(root.Name!=InvoiceNs+"Invoice")throw new ArgumentException("Document is not a supported UBL 2.1 invoice.");
        string Required(XName name,string label)=>root.Element(name)?.Value.Trim() is {Length:>0} value?value:throw new ArgumentException($"UBL invoice {label} is required.");
        var number=Required(Cbc+"ID","number");var dateText=Required(Cbc+"IssueDate","date");var currency=Required(Cbc+"DocumentCurrencyCode","currency").ToUpperInvariant();
        if(number.Length>100||currency.Length!=3||!DateOnly.TryParseExact(dateText,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out var date))throw new ArgumentException("UBL invoice identity is invalid.");
        var payable=root.Descendants(Cbc+"PayableAmount").LastOrDefault()??throw new ArgumentException("UBL invoice payable amount is required.");
        if(!decimal.TryParse(payable.Value,NumberStyles.Number,CultureInfo.InvariantCulture,out var amount)||amount<0)throw new ArgumentException("UBL invoice payable amount is invalid.");
        var orderReference=root.Descendants(Cac+"OrderReference").Elements(Cbc+"ID").FirstOrDefault()?.Value.Trim();
        return new(number,date,currency,amount,string.IsNullOrWhiteSpace(orderReference)?null:orderReference);
    }

    public ParsedSupplierOrderConfirmationUbl ParseOrderConfirmation(string xml)
    {
        if(string.IsNullOrWhiteSpace(xml)||xml.Length>2_000_000)throw new ArgumentException("UBL order confirmation must be between 1 byte and 2 MB.");
        using var reader=XmlReader.Create(new StringReader(xml),new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=2_000_000});
        var root=XDocument.Load(reader,LoadOptions.None).Root??throw new ArgumentException("UBL order confirmation XML is empty.");if(root.Name!=OrderResponseNs+"OrderResponse")throw new ArgumentException("Document is not a supported UBL 2.1 order confirmation.");
        var reference=root.Element(Cbc+"ID")?.Value.Trim();var orderReference=root.Descendants(Cac+"OrderReference").Elements(Cbc+"ID").FirstOrDefault()?.Value.Trim();var dateText=root.Descendants(Cac+"PromisedDeliveryPeriod").Elements(Cbc+"EndDate").FirstOrDefault()?.Value.Trim();
        if(string.IsNullOrWhiteSpace(reference)||reference.Length>250||!DateOnly.TryParseExact(dateText,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out var date))throw new ArgumentException("UBL order confirmation reference and promised delivery date are required.");
        var note=root.Element(Cbc+"Note")?.Value.Trim();return new(reference,string.IsNullOrWhiteSpace(orderReference)?null:orderReference,date,string.IsNullOrWhiteSpace(note)?null:note);
    }
}
