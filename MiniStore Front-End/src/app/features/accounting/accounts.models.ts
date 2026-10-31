export interface AccountRow {
  id: number;
  code: string;
  name: string;
  type: number;
  parentAccountId: number | null;
  parentAccountName: string | null;
  isActive: boolean;
}
export interface CreateAccount {
  code: string;
  name: string;
  type: number;
  parentAccountId: number | null;
}
export interface AccountPage {
  accounts: AccountRow[];
  form: CreateAccount;
}
