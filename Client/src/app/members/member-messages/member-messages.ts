import { AfterViewChecked, Component, inject, input, OnInit, output, ViewChild } from '@angular/core';
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
export class MemberMessages implements AfterViewChecked{
  @ViewChild('messageForm') messageForm?:NgForm;
  @ViewChild('scrollMe') scrollContainer?:any;
  username = input.required<string>();
  messageService = inject(MessageService);
  // messages = input.required<Message[]>();
  // updateMessage = output<Message>();
  messageContent= '';
  
  sendMessage(){
    this.messageService.sendMessage(this.messageContent,this.username()).then(() => {
      this.messageForm?.reset();
      this.scrollToBottom();
    });
  }
  
  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }
  
  private scrollToBottom(){
    if(this.scrollContainer){
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    }
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
