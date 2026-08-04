import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { MemberService } from '../../_services/member.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoPipe } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberMessages } from "../member-messages/member-messages";
import { Message } from '../../_models/message.model';
import { MessageService } from '../../_services/message.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-member-detail',
  imports: [TabsModule, GalleryModule, TimeagoPipe, DatePipe, MemberMessages],
  templateUrl: './member-detail.html',
  styleUrl: './member-detail.css',
})
export class MemberDetail implements OnInit {
  @ViewChild(TabsetComponent,{ static:true }) memberTabs? :TabsetComponent;
  private messageService = inject(MessageService);
  private memberService = inject(MemberService);
  private route = inject(ActivatedRoute);
  member = {} as Member;
  images: GalleryItem[] = [];
  messages:Message[] = [];
  activeTab? :TabDirective;

  ngOnInit(): void {
    // this.loadMember();
    
        this.route.data.subscribe({
          next: data => {
            this.member = data['member'];
            this.member && this.member.photos.map( p => {
              this.images.push(new ImageItem({src:p.url,thumb:p.url}))
            });
          }
        });

    this.route.queryParams.subscribe({
      next: qp => {
        qp['tab'] && this.activateMessagesTab(qp['tab']); 
      }
    });
    
  }

  activateMessagesTab(heading:string){
    if(this.memberTabs){
      const messagesTab = this.memberTabs.tabs.find(t => t.heading() === heading);
      if(messagesTab) messagesTab.active = true;
    }
  }
  

  onTabActivated(data:TabDirective){
    this.activeTab = data;
    // console.log('Inside onTabActivated');
    
    if(this.activeTab.heading() === 'Messages' && this.messages.length === 0 && this.member){
        this.messageService.getMessageThread(this.member.username).subscribe({
          next: (res) => {
            this.messages = res;
            // console.log("Respons is received.")
          }
    })
    }
  }

  onUpdateMessages(event:Message){
    this.messages.push(event);
  }

  // loadMember() {
  //   const username = this.route.snapshot.paramMap.get('username');
  //   if (!username) return;
  //   this.memberService.getMemberByUsername(username).subscribe({
  //     next: (res) => {
  //       this.member = res;
  //       // this.member?.photos.map((p) => {
  //       res.photos.map((p) => {
  //         this.images.push(
  //           new ImageItem({
  //             src: p.url,
  //             thumb: p.url,
  //           }),
  //         );
  //       });
  //     },
  //   });
  // }
}
