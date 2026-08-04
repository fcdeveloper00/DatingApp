import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { setPaginatedResult, setPaginationHeader } from './paginationHelper';
import { PaginatedResult } from '../_models/pagination';
import { Message } from '../_models/message.model';

@Injectable({
  providedIn: 'root',
})
export class MessageService {

  private baseUrl = environment.apiUrl;
  private httpClient = inject(HttpClient);
  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);

  getUserMessages(container: string, pageNumber: number, pageSize: number) {

    let params = setPaginationHeader(pageNumber,pageSize);
    params = params.append('container',container);

    this.httpClient.get<Message[]>(`${this.baseUrl}/messages`,{observe:'response',params}).subscribe({
      next: res => setPaginatedResult(res, this.paginatedResult)
    });
  }

  getMessageThread(usernameToChatWith:string){
    return this.httpClient.get<Message[]>(`${this.baseUrl}/messages/thread/${usernameToChatWith}`)
  }

  sendMessage(content:string,recipientUsername:string){
    return this.httpClient.post<Message>(`${this.baseUrl}/messages`,{content,recipientUsername:recipientUsername});
  }

  deleteMessage(id:number){
    return this.httpClient.delete(`${this.baseUrl}/messages/${id}`);
  }

}
