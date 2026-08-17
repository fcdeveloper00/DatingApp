import { Component, computed, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from '../../_models/member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoPipe } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberMessages } from "../member-messages/member-messages";
import { Message } from '../../_models/message.model';
import { MessageService } from '../../_services/message.service';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';
import { HubConnectionState } from '@microsoft/signalr';

@Component({
  selector: 'app-member-detail',
  imports: [TabsModule, GalleryModule, TimeagoPipe, DatePipe, MemberMessages],
  templateUrl: './member-detail.html',
  styleUrl: './member-detail.css',
})
export class MemberDetail implements OnInit, OnDestroy {
  @ViewChild(TabsetComponent,{ static:true }) memberTabs? :TabsetComponent;
  private messageService = inject(MessageService);
  private route = inject(ActivatedRoute);
  private accountService = inject(AccountService);
  private router = inject(Router);
  presenceService = inject(PresenceService);
  member = {} as Member;
  images: GalleryItem[] = [];
  // messages:Message[] = [];
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

        this.route.paramMap.subscribe({
          next: _ => this.onRouteParamsChange(),
        })

    this.route.queryParams.subscribe({
      next: qp => {
        qp['tab'] && this.activateMessagesTab(qp['tab']); 
      }
    });
    
  }
  
  onRouteParamsChange(){
    const user = this.accountService.currentUser();
    if(!user) return;
    if(this.messageService.hubConnection?.state === HubConnectionState.Connected && this.activeTab?.heading() === 'Messages'){
      this.messageService.hubConnection.stop().then(() => {
        this.messageService.createHubConnection(user,this.member.username);
      })
    }
  }
  
  activateMessagesTab(heading:string){
    if(this.memberTabs){
      const messagesTab = this.memberTabs.tabs.find(t => t.heading() === heading);
      if(messagesTab) messagesTab.active = true;
    }
  }
  

  onTabActivated(data:TabDirective){
    this.activeTab = data;
    this.router.navigate([], {
      // Populate(Update) the queryParams with whatever the 'activeTab.heading' is.
      relativeTo:this.route,
      queryParams:{
        tab:this.activeTab.heading()
      },
      queryParamsHandling:'merge'
    })
    if(this.activeTab.heading() === 'Messages' && this.member){
      const user = this.accountService.currentUser();
      if(!user) return;
      this.messageService.createHubConnection(user,this.member.username);
      
    }else{
      this.messageService.stopHubConnection();
    }
  }

  

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  // onUpdateMessages(event:Message){
  //   this.messages.push(event);
  // }

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
