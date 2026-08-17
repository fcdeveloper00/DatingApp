import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { User } from '../_models/user.model';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl;
  private httpClient = inject(HttpClient);


  getUsersWithRoles(){
    return this.httpClient.get<User[]>(`${this.baseUrl}/admin/users-with-roles`);
  }

  updateUserRoles(username:string, roles:string[]){
    // const rolesStr = roles.join(',');
    // let params = new HttpParams();
    // params = params.append ('roles',roles.toString());
    
    return this.httpClient.post<string[]>(`${this.baseUrl}/admin/edit-roles/${username}?roles=${roles}`,{});
  }
}
