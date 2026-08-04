import { Component, inject, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MemberService } from '../../_services/member.service';
import { MemberCard } from '../member-card/member-card';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { UserParams } from '../../_models/userParams';
import { AccountService } from '../../_services/account.service';
import { FormsModule } from '@angular/forms';
import { HttpParams } from '@angular/common/http';
import {ButtonsModule} from 'ngx-bootstrap/buttons'

@Component({
  selector: 'app-member-list',
  imports: [MemberCard, PaginationModule,FormsModule,ButtonsModule],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css',
})
export class MemberList implements OnInit {
  members?: Member[];
  memberService = inject(MemberService);
  accountService = inject(AccountService);
  // userParams = new UserParams(this.accountService.currentUser());
  genderList = [
    {value:'male', display:'Males'},
    {value:'female', display:'Females'}
  ]

  ngOnInit() {
    if (!this.memberService.paginatedResult()) this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers();
  }

  resetFilters(){
    // this.memberService.userParams.set(new UserParams(this.accountService.currentUser()));
    this.memberService.resetUserParams();
    this.loadMembers();
  }

  pageChanged(event: any) {
    console.log(event);

    if (this.memberService.userParams().pageNumber !== event.page) {
      this.memberService.userParams().pageNumber = event.page;
      this.loadMembers();
    }
  }
}
