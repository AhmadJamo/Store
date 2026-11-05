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
  tree: AccountTreeNode[];
  form: CreateAccount;
}
export interface AccountTreeNode {
  id: number;
  code: string;
  name: string;
  type: number;
  isActive: boolean;
  children: AccountTreeNode[];
}
