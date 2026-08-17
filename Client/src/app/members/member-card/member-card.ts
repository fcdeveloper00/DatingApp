import { Component, computed, inject, input } from '@angular/core';
import { Member } from '../../_models/member';
import { RouterLink } from "@angular/router";
import { LikesService } from '../../_services/likes.service';
import { PresenceService } from '../../_services/presence.service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink],
  templateUrl: './member-card.html',
  styleUrl: './member-card.css',
})
export class MemberCard {
  member = input.required<Member>();
  presenceService = inject(PresenceService);
  private likesService = inject(LikesService);
  hasLiked = computed(() => this.likesService.likeIds().includes(this.member().id));
  isOnline = computed(() => this.presenceService.onlineUsers()?.includes(this.member().username));
  
  toggleUserLike(targetUserId:number){
    this.likesService.toggleUserLike(targetUserId).subscribe({
      next: () => {
        if(this.hasLiked()){
          this.likesService.likeIds.update(ids => 
            this.likesService.likeIds().filter(id => id !== targetUserId)
          )
        }else{
          this.likesService.likeIds.update(ids => [...ids,targetUserId])
        }
      },
    });
  }


}