import { Component, inject, input, OnInit, output, ViewChild } from '@angular/core';
import { MessageService } from '../../_services/message.service';
import { Message } from '../../_models/message.model';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from "@angular/forms";

@Component({
  selector: 'app-member-messages',
  imports: [TimeagoModule, FormsModule],
  templateUrl: './member-messages.html',
  styleUrl: './member-messages.css',
})
export class MemberMessages {
  @ViewChild('messageForm') messageForm?:NgForm;
  username = input.required<string>();
  messageService = inject(MessageService);
  // messages = input.required<Message[]>();
  // updateMessage = output<Message>();
  messageContent= '';
  
  sendMessage(){
    this.messageService.sendMessage(this.messageContent,this.username()).then(() => {
      this.messageForm?.reset();
    });
  }

  
  
  
  
  // ngOnInit(): void {
  //   // this.loadMemberMessages();
  // }

  // loadMemberMessages(){
  //   this.messageService.getMessageThread(this.username()).subscribe({
  //     next: res => this.messages = res,
  //   })
  // }
}
