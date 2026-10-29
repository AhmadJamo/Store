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
