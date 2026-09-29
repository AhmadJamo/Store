using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Application.Services;

public sealed record JournalPostingRequest(
    DateTime Date,
    string Description,
    string SourceType,
    string SourceReference,
    IReadOnlyCollection<JournalEntryLine> Lines,
    string DuplicateMessage);

public sealed class JournalPostingService(
    IJournalEntryRepository journalEntryRepository,
    DocumentNumberService documentNumbers,
    FiscalPeriodService fiscalPeriods)
{
    public Task<bool> IsPostedAsync(string sourceType, string sourceReference) =>
        journalEntryRepository.ExistsForSourceAsync(sourceType, sourceReference);

    public async Task<JournalEntry> PostAsync(JournalPostingRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SourceType) ||
            string.IsNullOrWhiteSpace(request.SourceReference))
            throw new ArgumentException("A journal source requires both its type and reference.");
        if (request.Lines.Count < 2)
            throw new InvalidOperationException("A posting requires at least two journal lines.");
        await fiscalPeriods.EnsurePostingAllowedAsync(request.Date);
        if (await IsPostedAsync(request.SourceType, request.SourceReference))
            throw new InvalidOperationException(request.DuplicateMessage);

        var entry = new JournalEntry(
            await documentNumbers.GenerateAsync(DocumentNumberType.JournalEntry, request.Date),
            request.Date,
            request.Description,
            request.SourceType,
            request.SourceReference);
        foreach (var line in request.Lines)
            entry.AddLine(line);

        entry.Post();
        await journalEntryRepository.AddAsync(entry);
        return entry;
    }
}
