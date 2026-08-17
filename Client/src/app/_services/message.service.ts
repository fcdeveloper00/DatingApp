import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { setPaginatedResult, setPaginationHeader } from './paginationHelper';
import { PaginatedResult } from '../_models/pagination';
import { Message } from '../_models/message.model';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../_models/user.model';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  hubUrl = environment.hubsUrl;
  private baseUrl = environment.apiUrl;
  private httpClient = inject(HttpClient);
  hubConnection? : HubConnection;
  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);
  messageThread  = signal<Message[]> ([]);

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

  async sendMessage(content:string,recipientUsername:string){
    // return this.httpClient.post<Message>(`${this.baseUrl}/messages`,{content,recipientUsername:recipientUsername});
    return this.hubConnection?.invoke("SendMessage",{recipientUsername,content})
  }

  deleteMessage(id:number){
    return this.httpClient.delete(`${this.baseUrl}/messages/${id}`);
  }

  createHubConnection(user:User, otherUsername:string){
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}/message?user=${otherUsername}`, {
        accessTokenFactory : () => user.token,
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection.start().catch(err => console.log(err));

      this.hubConnection.on("ReceiveMessageThread", messages => {
        this.messageThread.set(messages);
      });
      
      this.hubConnection.on("NewMessage", (newMessage) => {
        this.messageThread.update( self =>[...self, newMessage])
      });
      
      this.hubConnection.on("UpdatedGroup",(group:Group) => {
        if(group.connections.some(c => c.username === otherUsername)){
          this.messageThread.update(self => {
            self.forEach(m => {
              m.readAt ??= new Date(Date.now());
              // if(!m.readAt)
              //   m.readAt = new Date(Date.now());
            });
            return self;
          })
        }
      });
  }

  stopHubConnection(){
    if(this.hubConnection?.state === HubConnectionState.Connected){
      this.hubConnection.stop().catch(err => console.log(err));
    }
  }
}
