import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AccountPage, CreateAccount } from './accounts.models';
@Injectable({ providedIn: 'root' })
export class AccountsApiService {
  private readonly base = '/api/v1/accounts';
  constructor(private readonly http: HttpClient) {}
  get() {
    return this.http.get<AccountPage>(this.base);
  }
  create(input: CreateAccount) {
    return this.http.post<void>(this.base, input);
  }
}
