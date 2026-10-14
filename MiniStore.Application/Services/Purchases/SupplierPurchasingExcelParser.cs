using ClosedXML.Excel;
using System.Globalization;
using System.IO.Compression;

namespace MiniStore.Application.Services;

public static class SupplierPurchasingExcelParser
{
    public const string WorksheetName = "SupplierPrices";
    private const long MaximumExpandedBytes = 20 * 1024 * 1024;
    private const int MaximumArchiveEntries = 200;

    public static byte[] Write(IEnumerable<IReadOnlyList<string>> rows)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(WorksheetName);
        worksheet.ShowGridLines = false;

        for (var column = 0; column < SupplierPurchasingCsvParser.Headers.Length; column++)
            worksheet.Cell(1, column + 1).Value = SupplierPurchasingCsvParser.Headers[column];

        var rowNumber = 2;
        foreach (var row in rows)
        {
            if (row.Count != SupplierPurchasingCsvParser.Headers.Length)
                throw new ArgumentException("One or more supplier purchasing rows contain an invalid number of columns.");
            for (var column = 0; column < row.Count; column++)
                WriteCell(worksheet.Cell(rowNumber, column + 1), column, row[column]);
            rowNumber++;
        }

        var lastRow = Math.Max(1, rowNumber - 1);
        var usedRange = worksheet.Range(1, 1, lastRow, SupplierPurchasingCsvParser.Headers.Length);
        var header = worksheet.Range(1, 1, 1, SupplierPurchasingCsvParser.Headers.Length);
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
        header.Style.Font.FontColor = XLColor.White;
        header.Style.Font.Bold = true;
        header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        header.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        header.Style.Border.BottomBorderColor = XLColor.White;
        worksheet.SheetView.FreezeRows(1);
        usedRange.SetAutoFilter();
        usedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        worksheet.Column(1).Width = 18;
        worksheet.Column(2).Width = 18;
        worksheet.Column(3).Width = 22;
        worksheet.Column(4).Width = 34;
        worksheet.Columns(5, 12).Width = 18;
        worksheet.Column(9).Style.DateFormat.Format = "yyyy-mm-dd";
        worksheet.Column(10).Style.DateFormat.Format = "yyyy-mm-dd";
        worksheet.Range(2, 11, 501, 11).CreateDataValidation().List("\"TRUE,FALSE\"");

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public static List<IReadOnlyDictionary<string, string>> Parse(byte[] content)
    {
        if (content.Length == 0) throw new ArgumentException("The Excel file is empty.");
        ValidateArchive(content);

        try
        {
            using var stream = new MemoryStream(content, writable: false);
            using var workbook = new XLWorkbook(stream);
            if (!workbook.Worksheets.TryGetWorksheet(WorksheetName, out var worksheet))
                throw new ArgumentException("The Excel file does not contain the SupplierPrices worksheet.");

            if ((worksheet.LastColumnUsed()?.ColumnNumber() ?? 0) > SupplierPurchasingCsvParser.Headers.Length)
                throw new ArgumentException("The Excel columns do not match the supplier purchasing template.");
            var headers = Enumerable.Range(1, SupplierPurchasingCsvParser.Headers.Length)
                .Select(column => ReadCell(worksheet.Cell(1, column)).Trim()).ToArray();
            if (!headers.SequenceEqual(SupplierPurchasingCsvParser.Headers, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("The Excel columns do not match the supplier purchasing template.");

            var lastUsedRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
            if (lastUsedRow == 1) throw new ArgumentException("The Excel file does not contain purchasing rows.");
            if (lastUsedRow > 501) throw new ArgumentException("A supplier purchasing import cannot exceed 500 rows.");

            var rows = new List<IReadOnlyDictionary<string, string>>();
            for (var rowNumber = 2; rowNumber <= lastUsedRow; rowNumber++)
            {
                var values = Enumerable.Range(1, SupplierPurchasingCsvParser.Headers.Length)
                    .Select(column => ReadCell(worksheet.Cell(rowNumber, column))).ToArray();
                if (values.All(string.IsNullOrWhiteSpace)) continue;
                rows.Add(SupplierPurchasingCsvParser.Headers.Select((header, index) => new { header, value = values[index].Trim() })
                    .ToDictionary(x => x.header, x => x.value, StringComparer.OrdinalIgnoreCase));
            }
            if (rows.Count == 0) throw new ArgumentException("The Excel file does not contain purchasing rows.");
            return rows;
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception exception) when (exception is InvalidDataException or NotSupportedException or FormatException)
        {
            throw new ArgumentException("The Excel file could not be read.", exception);
        }
    }

    private static void WriteCell(IXLCell cell, int columnIndex, string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        switch (columnIndex)
        {
            case 4 or 5 or 7 when decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number):
                cell.Value = number;
                cell.Style.NumberFormat.Format = "0.######";
                break;
            case 6 or 11 when int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer):
                cell.Value = integer;
                cell.Style.NumberFormat.Format = "0";
                break;
            case 8 or 9 when DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date):
                cell.Value = date;
                cell.Style.DateFormat.Format = "yyyy-mm-dd";
                break;
            case 10 when bool.TryParse(value, out var boolean):
                cell.Value = boolean;
                break;
            default:
                cell.Value = value;
                cell.Style.NumberFormat.Format = "@";
                break;
        }
    }

    private static string ReadCell(IXLCell cell)
    {
        if (cell.HasFormula) throw new ArgumentException("Excel formulas are not allowed in supplier purchasing imports.");
        if (cell.IsEmpty()) return string.Empty;
        return cell.DataType switch
        {
            XLDataType.Boolean => cell.GetBoolean() ? "TRUE" : "FALSE",
            XLDataType.Number => cell.GetDouble().ToString("0.############################", CultureInfo.InvariantCulture),
            XLDataType.DateTime => cell.GetDateTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            XLDataType.TimeSpan => cell.GetTimeSpan().ToString("c", CultureInfo.InvariantCulture),
            _ => cell.GetString()
        };
    }

    private static void ValidateArchive(byte[] content)
    {
        try
        {
            using var stream = new MemoryStream(content, writable: false);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
            if (archive.Entries.Count == 0 || archive.Entries.Count > MaximumArchiveEntries)
                throw new ArgumentException("The Excel file has an invalid or unsafe structure.");
            long expandedBytes = 0;
            foreach (var entry in archive.Entries)
            {
                expandedBytes = checked(expandedBytes + entry.Length);
                if (entry.Length > MaximumExpandedBytes || expandedBytes > MaximumExpandedBytes)
                    throw new ArgumentException("The Excel file has an invalid or unsafe structure.");
            }
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception exception) when (exception is InvalidDataException or OverflowException)
        {
            throw new ArgumentException("The Excel file could not be read.", exception);
        }
    }
}
