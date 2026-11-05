import { JournalEntry, JournalEntryListPage } from './journal-entry.models';
export interface ManualJournalOption { id:number; display:string; }
export interface ManualJournalCreatePage { accounts:ManualJournalOption[]; branches:ManualJournalOption[]; warehouses:ManualJournalOption[]; }
export interface ManualJournalLineInput { accountId:number; branchId:number|null; warehouseId:number|null; debit:number; credit:number; description:string|null; }
export interface ManualJournalDraft { date:string; description:string; lines:ManualJournalLineInput[]; idempotencyKey:string; }
export type CreateManualJournal = ManualJournalDraft;
export interface ReverseManualJournal { date:string; reason:string; idempotencyKey:string; }
export type ManualJournal = JournalEntry;
export type ManualJournalListPage = JournalEntryListPage;
