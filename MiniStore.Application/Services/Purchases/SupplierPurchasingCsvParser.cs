using System.Text;

namespace MiniStore.Application.Services;

public static class SupplierPurchasingCsvParser
{
    public static readonly string[] Headers =
    [
        "ProductCode", "PurchaseUnitCode", "SupplierProductCode", "SupplierDescription",
        "MinimumOrderQuantity", "OrderMultiple", "LeadTimeDays", "UnitPrice",
        "ValidFrom", "ValidTo", "IsPreferred", "Priority"
    ];

    public static List<IReadOnlyDictionary<string, string>> Parse(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("The CSV file is empty.");
        var rows = ParseRows(content.TrimStart('\uFEFF'));
        if (rows.Count == 0) throw new ArgumentException("The CSV file is empty.");
        var headers = rows[0].Select(x => x.Trim()).ToArray();
        if (headers.Length != Headers.Length || !headers.SequenceEqual(Headers, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException("The CSV columns do not match the supplier purchasing template.");
        if (rows.Count == 1) throw new ArgumentException("The CSV file does not contain purchasing rows.");
        if (rows.Count > 501) throw new ArgumentException("A supplier purchasing import cannot exceed 500 rows.");

        return rows.Skip(1).Where(row => row.Any(value => !string.IsNullOrWhiteSpace(value))).Select(row =>
        {
            if (row.Count != Headers.Length) throw new ArgumentException("One or more CSV rows contain an invalid number of columns.");
            return (IReadOnlyDictionary<string, string>)Headers.Select((header, index) => new { header, value = Unprotect(row[index].Trim()) })
                .ToDictionary(x => x.header, x => x.value, StringComparer.OrdinalIgnoreCase);
        }).ToList();
    }

    public static string Write(IEnumerable<IReadOnlyList<string>> rows)
    {
        var builder = new StringBuilder();
        builder.AppendLine(string.Join(',', Headers));
        foreach (var row in rows)
            builder.AppendLine(string.Join(',', row.Select(Escape)));
        return builder.ToString();
    }

    private static List<List<string>> ParseRows(string content)
    {
        var rows = new List<List<string>>();
        var row = new List<string>();
        var field = new StringBuilder();
        var quoted = false;
        for (var index = 0; index < content.Length; index++)
        {
            var character = content[index];
            if (quoted)
            {
                if (character == '"' && index + 1 < content.Length && content[index + 1] == '"') { field.Append('"'); index++; }
                else if (character == '"') quoted = false;
                else field.Append(character);
                continue;
            }
            if (character == '"' && field.Length == 0) quoted = true;
            else if (character == ',') { row.Add(field.ToString()); field.Clear(); }
            else if (character is '\r' or '\n')
            {
                if (character == '\r' && index + 1 < content.Length && content[index + 1] == '\n') index++;
                row.Add(field.ToString()); field.Clear(); rows.Add(row); row = [];
            }
            else field.Append(character);
        }
        if (quoted) throw new ArgumentException("The CSV file contains an unclosed quoted value.");
        if (field.Length > 0 || row.Count > 0) { row.Add(field.ToString()); rows.Add(row); }
        return rows;
    }

    private static string Escape(string value)
    {
        var safe = value;
        if (safe.Length > 0 && safe[0] is '=' or '+' or '-' or '@') safe = "'" + safe;
        return safe.ContainsAny(',', '"', '\r', '\n') ? $"\"{safe.Replace("\"", "\"\"")}\"" : safe;
    }

    private static string Unprotect(string value) => value.Length > 1 && value[0] == '\'' && value[1] is '=' or '+' or '-' or '@'
        ? value[1..]
        : value;

    private static bool ContainsAny(this string value, params char[] characters) => value.IndexOfAny(characters) >= 0;
}
