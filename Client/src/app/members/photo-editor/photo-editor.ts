import { Component, inject, input, OnInit, output } from '@angular/core';
import { Member } from '../../_models/member';
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { NgForm } from '@angular/forms';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { MemberService } from '../../_services/member.service';
import { Photo } from '../../_models/photo';

@Component({
  selector: 'app-photo-editor',
  imports: [NgIf, NgFor, NgClass, NgStyle, DecimalPipe, FileUploadModule],
  templateUrl: './photo-editor.html',
  styleUrl: './photo-editor.css',
})
export class PhotoEditor implements OnInit {
  private accountService = inject(AccountService);
  private memberService = inject(MemberService);
  member = input.required<Member>();
  uploader?: FileUploader;
  baserUrl = environment.apiUrl;
  hasBaseDropZoneOver = false;
  //We actually need to update the 'member' in our parent component
  //and that change gets pushed down to this child component as its input property.
  memberChange = output<Member>();

  //https://randomuser.me/api/portraits/men/90.jpg

  ngOnInit(): void {
    this.intializeUploader();
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo).subscribe({
      next: (_) => {
        const user = this.accountService.currentUser();
        if (user) {
          user.photoUrl = photo.url;
          this.accountService.setCurrentUser(user);
        }

        const updatedMember = { ...this.member() };
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
        });

        this.memberChange.emit(updatedMember);
      },
    });
  }

  deletePhoto(photo:Photo) {
    this.memberService.deletePhoto(photo).subscribe({
      next: (_) => {
        const updatedMember = { ...this.member() };
        if (updatedMember) {
          updatedMember.photos = updatedMember.photos.filter((p) => p.id !== photo.id);
          this.memberChange.emit(updatedMember);
        }
      },
    });
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  intializeUploader() {
    this.uploader = new FileUploader({
      url: `${this.baserUrl}/users/add-photo`,
      authToken: `Bearer ${this.accountService.currentUser()?.token}`,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      const photo = JSON.parse(response);
      const updatedMember = { ...this.member() };
      updatedMember.photos.push(photo);
      
      if(photo.isMain){
        const user = this.accountService.currentUser();
        if(user){
          user.photoUrl = photo.url;
          this.accountService.setCurrentUser(user);
        }
        updatedMember.photoUrl= photo.url;
        this.memberChange.emit(updatedMember);
      }
      this.memberChange.emit(updatedMember);
    };
  }
}
