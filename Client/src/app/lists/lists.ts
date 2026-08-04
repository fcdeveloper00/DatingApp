import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { LikesService } from '../_services/likes.service';
import { Member } from '../_models/member';
import { ButtonRadioDirective } from "ngx-bootstrap/buttons";
import { FormsModule } from '@angular/forms';
import { MemberCard } from "../members/member-card/member-card";
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-lists',
  imports: [ButtonRadioDirective, FormsModule, MemberCard,PaginationModule],
  templateUrl: './lists.html',
  styleUrl: './lists.css',
})
export class Lists implements OnInit, OnDestroy{
  likesService = inject(LikesService);
  // members: Member[] = [];
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 2;

  ngOnInit(): void {
    this.getUserLikes();
  }

  getUserLikes() {
    this.likesService.getUserLikes(this.predicate,this.pageNumber,this.pageSize);
  }

  getTitle() {
    switch (this.predicate) {
      case 'liked':
        return 'Members you like';
      case 'likedBy':
        return 'Members who like you';
      default:
        return 'Mutual';
    }
  }

  pageChanged(event:any){
    if(this.pageNumber !== event.page){
      this.pageNumber = event.page;
      this.getUserLikes();
    }
  }
  
  ngOnDestroy(): void {
    this.likesService.paginatedResult.set(null);
  }  

}
