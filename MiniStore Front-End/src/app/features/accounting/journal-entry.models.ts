export interface JournalLine {
  accountDisplay: string;
  branchName: string | null;
  warehouseName: string | null;
  debit: number;
  credit: number;
  description: string | null;
}
export interface JournalEntry {
  id: number;
  entryNumber: string;
  date: string;
  description: string | null;
  sourceType: string | null;
  sourceReference: string | null;
  status: number;
  reversalOfEntryId: number | null;
  lines: JournalLine[];
  totalDebit: number;
  totalCredit: number;
}
export interface JournalEntryListQuery { search: string; fromDate: string; toDate: string; status: 'all' | '1' | '2' | '3'; page: number; pageSize: 10 | 25 | 50 | 100; }
export interface JournalEntryListItem { id: number; entryNumber: string; date: string; description: string | null; sourceType: string | null; sourceReference: string | null; status: number; totalDebit: number; totalCredit: number; }
export interface JournalEntryListPage { query: unknown; items: JournalEntryListItem[]; totalCount: number; totalPages: number; }
