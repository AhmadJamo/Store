export interface ReplenishmentSuggestion {
  id: number;
  ruleId: number;
  product: string;
  warehouse: string;
  available: number;
  reserved: number;
  incoming: number;
  historicalDemand: number;
  seasonalFactor: number;
  target: number;
  suggestedStock: number;
  suggestedPurchase: number;
  purchaseUnitSymbol: string;
  currencyCode: string;
  neededByDate: string;
  outcome: number;
  purchaseRequestId?: number;
  purchaseSourcingEventId?: number;
  reasonCode: number;
}
export interface ReplenishmentPage {
  settings: {
    isEnabled: boolean;
    frequencyHours: number;
    forecastLookbackDays: number;
    mode: number;
    supplierResponseDays: number;
    rowVersion: string;
  };
  lastRunAtUtc?: string;
  nextRunAtUtc?: string;
  runs: {
    id: number;
    startedAtUtc: string;
    completedAtUtc?: string;
    mode: number;
    status: number;
    isScheduled: boolean;
    triggeredByUserId: string;
    error?: string;
    suggestions: ReplenishmentSuggestion[];
  }[];
}
