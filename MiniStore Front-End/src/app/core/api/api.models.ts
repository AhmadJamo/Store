export interface ApiTenant {
  id: number;
  name: string;
  code: string;
}

export interface ApiUser {
  id: string;
  userName: string | null;
  email: string | null;
}

export interface ApiSession {
  user: ApiUser;
  tenant: ApiTenant;
  culture: 'ar-JO' | 'en-US' | string;
  direction: 'rtl' | 'ltr';
  permissions: readonly string[];
}

export interface AntiforgeryToken {
  token: string;
  headerName: string;
}

export interface ApiProblem {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  correlationId?: string;
  code?: string;
  errors?: Record<string, readonly string[]>;
}

export type ApiProblemKind =
  | 'validation'
  | 'duplicate'
  | 'concurrency'
  | 'notFound'
  | 'unauthorized'
  | 'forbidden'
  | 'transient'
  | 'unknown';

export interface NormalizedApiProblem extends ApiProblem {
  kind: ApiProblemKind;
  canRetry: boolean;
}
