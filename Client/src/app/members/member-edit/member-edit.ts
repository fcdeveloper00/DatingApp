import { Component, HostListener, inject, OnInit, ViewChild } from '@angular/core';
import { Member } from '../../_models/member';
import { AccountService } from '../../_services/account.service';
import { MemberService } from '../../_services/member.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PhotoEditor } from "../photo-editor/photo-editor";
import { TimeagoPipe } from 'ngx-timeago';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-member-edit',
  imports: [TabsModule, FormsModule, PhotoEditor,TimeagoPipe,DatePipe],
  templateUrl: './member-edit.html',
  styleUrl: './member-edit.css',
})
export class MemberEdit implements OnInit {
  @ViewChild('editForm') updateForm?: NgForm;
  @HostListener('window:beforeunload', ['$event']) nofifyUser($event: any) {
    if (this.updateForm?.dirty) {
      $event.returnValue = true;
    }
    // return $event.
  }
  member?: Member;
  private accountService = inject(AccountService);
  private memberService = inject(MemberService);
  private toastr = inject(ToastrService);

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    const user = this.accountService.currentUser();
    if (!user) return;
    this.memberService.getMemberByUsername(user.userName).subscribe({
      next: (res) => (this.member = res),
      error: (e) => console.log(e),
    });
  }

  updateMember() {
    console.log(this.member);
    if(this.member){

      this.memberService.updateMember(this.member).subscribe({
        next: (res) => {
          // this.member = res;
          this.toastr.success('profile updated successfully.');
          console.log(res);
          this.updateForm?.reset(this.member);
        },
        error: (e) => this.toastr.error(e),
      });
    }else{
      this.toastr.error("You Can't update an empty person");
    }
  }

  onMemberChange(e:Member){
    this.member = e;
  }
}
