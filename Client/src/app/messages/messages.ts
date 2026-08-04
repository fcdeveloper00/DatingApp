import { Component, inject, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from '../_models/message.model';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-messages',
  imports: [ButtonsModule,FormsModule,PaginationModule,TimeagoModule,RouterLink],
  templateUrl: './messages.html',
  styleUrl: './messages.css',
})
export class Messages implements OnInit {
  messageService = inject(MessageService);
  router = inject(Router);
  pageNumber = 1;
  pageSize = 3;
  container = 'Outbox';
  isOutbox = this.container === 'Outbox';

  ngOnInit(): void {
    this.loadMessages();
  }
  getObject(){
    return {tab:'messages'}
  }

  onDeleteMessage(id:number){
    this.messageService.deleteMessage(id).subscribe({
      next: _ => {
        this.messageService.paginatedResult.update(pr => {
          if( pr?.items && pr?.items?.length > 0){
 
            // pr.items = pr?.items.filter(m => m.id !== id)
            // return pr;
            pr.items.splice(pr.items.findIndex(m => m.id === id),1)
            return pr;
          }
            return pr;
        })
      }
    })
  }

  loadMessages() {
    this.messageService.getUserMessages(
      this.container, this.pageNumber, this.pageSize);
  }

  getRoute(message:Message){
    if(this.container === 'Inbox'){
      return `/members/${message.senderUsername}`;
    }else{
      return `/members/${message.recipientUsername}`;
    }
  }
  
  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }
}
