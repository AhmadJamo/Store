using MiniStore.Application.DTOs.Settings;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public class DocumentNumberService(IDocumentSequenceRepository repository)
{
    public async Task<DocumentNumberSettingsPageDto> GetPageAsync()
    {
        await EnsureDefaultsAsync();
        var rows = await repository.GetAllAsync();
        return new DocumentNumberSettingsPageDto
        {
            Sequences = rows.Select(Map).ToList()
        };
    }

    public async Task UpdateAsync(DocumentNumberSettingsPageDto dto)
    {
        if (dto?.Sequences is null)
            throw new ArgumentException("Document sequences are required.");

        var supportedTypes = Enum.GetValues<DocumentNumberType>();
        if (dto.Sequences.Count != supportedTypes.Length ||
            dto.Sequences.Any(x => !Enum.IsDefined(x.DocumentType)))
            throw new ArgumentException("Submit exactly one row for every supported document type.");

        var submitted = dto.Sequences
            .GroupBy(x => x.DocumentType)
            .ToDictionary(x => x.Key, x => x.Single());
        var rows = await repository.GetAllAsync();

        foreach (var type in supportedTypes)
        {
            if (!submitted.TryGetValue(type, out var input))
                throw new ArgumentException($"Settings for {type} are required.");

            var row = rows.SingleOrDefault(x => x.DocumentType == type);
            if (row is null)
            {
                row = DocumentSequence.CreateDefault(type);
                await repository.AddAsync(row);
            }
            else if (input.RowVersion.Length == 0 || !row.RowVersion.SequenceEqual(input.RowVersion))
            {
                throw new InvalidOperationException("Document numbering settings changed in another session. Reload and try again.");
            }

            row.Configure(input.Prefix, input.Suffix, input.FormatTemplate, input.NumberLength, input.ResetPeriod, input.ResetStartNumber);
            row.MoveNextNumberForward(input.NextNumber);
        }

        await repository.SaveChangesAsync();
    }

    public async Task<string> GenerateAsync(DocumentNumberType type, DateTime documentDate)
    {
        var sequence = await repository.GetAsync(type);
        if (sequence is null)
        {
            sequence = DocumentSequence.CreateDefault(type);
            await repository.AddAsync(sequence);
        }
        return sequence.GenerateNext(documentDate);
    }

    public async Task EnsureDefaultsAsync()
    {
        var existing = (await repository.GetAllAsync()).Select(x => x.DocumentType).ToHashSet();
        var added = false;
        foreach (var type in Enum.GetValues<DocumentNumberType>().Where(type => !existing.Contains(type)))
        {
            await repository.AddAsync(DocumentSequence.CreateDefault(type));
            added = true;
        }
        if (added) await repository.SaveChangesAsync();
    }

    private static DocumentSequenceDto Map(DocumentSequence row)
    {
        var names = row.DocumentType switch
        {
            DocumentNumberType.WholesaleSale => ("Wholesale sales invoice", "فاتورة مبيعات جملة"),
            DocumentNumberType.PosSale => ("POS sales invoice", "فاتورة نقطة بيع"),
            DocumentNumberType.StockTransfer => ("Stock transfer", "تحويل مخزني"),
            DocumentNumberType.JournalEntry => ("Journal entry", "قيد يومية"),
            _ => (row.DocumentType.ToString(), row.DocumentType.ToString())
        };
        return new DocumentSequenceDto
        {
            DocumentType = row.DocumentType,
            NameEnglish = names.Item1,
            NameArabic = names.Item2,
            Prefix = row.Prefix,
            Suffix = row.Suffix,
            FormatTemplate = row.FormatTemplate,
            NumberLength = row.NumberLength,
            NextNumber = row.NextNumber,
            ResetStartNumber = row.ResetStartNumber,
            ResetPeriod = row.ResetPeriod,
            Preview = row.Preview(DateTime.Today),
            RowVersion = row.RowVersion
        };
    }
}
