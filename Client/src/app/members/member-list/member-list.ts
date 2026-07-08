import { Component, inject, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MemberService } from '../../_services/member.service';
import { MemberCard } from '../member-card/member-card';

@Component({
  selector: 'app-member-list',
  imports: [MemberCard],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css',
})
export class MemberList implements OnInit{

members:Member[]= [];
private memberService = inject(MemberService);

ngOnInit(){
  this.loadMembers();
}

loadMembers(){
  this.memberService.getMembers().subscribe({
    next: response => this.members = response,
    error: e => console.log(e),
    complete: () => console.log('Members have been fetched Successfully!')
  })
}


}
