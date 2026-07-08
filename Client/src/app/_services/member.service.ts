import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Member } from '../_models/member';
import { environment } from '../../environments/environment';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  // private accountService = inject(AccountService);

  getMembers() {
    return this.http.get<Member[]>(`${this.baseUrl}/users`);
  }

  getMemberById(id: number) {
    return this.http.get<Member>(`${this.baseUrl}/users/${id}`);
  }

  getMemberByUsername(username: string) {
    return this.http.get<Member>(`${this.baseUrl}/users/${username}`);
  }

  // getHttpRequestHeaders() {
  //   return {
  //     headers: new HttpHeaders({
  //       Authorization: `Bearer ${this.accountService.currentUser()?.token}`,
  //     }),
  //   };
  // }
}
