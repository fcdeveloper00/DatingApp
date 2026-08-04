import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { setPaginatedResult, setPaginationHeader } from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  baseUrl = environment.apiUrl;
  private httpClient = inject(HttpClient);
  likeIds = signal<number[]>([]);
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);

  toggleUserLike(targetUserId: number) {
    // if (!this.likeIds().includes(targetUserId)) {
    //   this.likeIds.set([...this.likeIds(), targetUserId]);
    // } else {
    //   this.likeIds.set(this.likeIds().filter((id) => id !== targetUserId));

    //   this.likeIds.set([...this.likeIds()]);
    // }

    return this.httpClient.post(`${this.baseUrl}/likes/${targetUserId}`, {});
  }

  getUserLikes(predicate: string,pageNumber:number,pageSize:number) {
    let params = setPaginationHeader(pageNumber,pageSize);
    params =  params.append('predicate',predicate);
    
    return this.httpClient.get<Member[]>(`${this.baseUrl}/likes`,{observe:'response',params}).subscribe({
      next: res => setPaginatedResult(res, this.paginatedResult)
    });
  }

  getUserLikeIds() {
    return this.httpClient.get<number[]>(`${this.baseUrl}/likes/list`).subscribe({
      next: (res) => this.likeIds.set(res),
    });
  }
}
