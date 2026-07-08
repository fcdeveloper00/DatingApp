import { Component, inject, OnInit } from '@angular/core';
import { MemberService } from '../../_services/member.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../../_models/member';
import { TabsModule } from 'ngx-bootstrap/tabs';
import{GalleryItem, GalleryModule, ImageItem} from'ng-gallery';

@Component({
  selector: 'app-member-detail',
  imports: [TabsModule,GalleryModule],
  templateUrl: './member-detail.html',
  styleUrl: './member-detail.css',
})
export class MemberDetail implements OnInit{
  private memberService = inject(MemberService);
  private route = inject(ActivatedRoute);
  member?:Member;
  images :GalleryItem[] = [];

ngOnInit(): void {
  this.loadMember();
}

loadMember(){
  const username = this.route.snapshot.paramMap.get('username');
  if(!username) return;
  this.memberService.getMemberByUsername(username).subscribe({
    next:res => {
      this.member = res
      this.member.photos.map(p => {
        this.images.push(
          new ImageItem({
            src:p.url,thumb:p.url
          }));
      })
    }
  });
}


}
