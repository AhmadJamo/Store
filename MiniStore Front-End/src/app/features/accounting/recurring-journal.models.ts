import { ManualJournalCreatePage, ManualJournalLineInput } from './manual-journal.models';
export interface RecurringJournalTemplate { id:number; name:string; description:string; frequency:number; startDate:string; endDate:string|null; nextRunDate:string|null; isActive:boolean; lastRunAtUtc:string|null; lastRunDate:string|null; lastJournalEntryId:number|null; rowVersion:string; lines:(ManualJournalLineInput & {accountDisplay?:string;branchName?:string;warehouseName?:string})[]; }
export interface RecurringJournalPage { templates:RecurringJournalTemplate[]; options:ManualJournalCreatePage; }
export interface SaveRecurringJournalTemplate { name:string; description:string; frequency:number; startDate:string; endDate:string|null; lines:ManualJournalLineInput[]; rowVersion:string|null; }
export interface RecurringJournalRunResult { journalEntryId:number; runDate:string; nextRunDate:string|null; }
