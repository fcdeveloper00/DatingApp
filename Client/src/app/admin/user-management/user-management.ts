import { Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { User } from '../../_models/user.model';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModal } from '../../modals/roles-modal/roles-modal';
import { Title } from '@angular/platform-browser';
import { listLocales } from 'ngx-bootstrap/chronos';

@Component({
  selector: 'app-user-management',
  imports: [],
  templateUrl: './user-management.html',
  styleUrl: './user-management.css',
})
export class UserManagement implements OnInit {
  private adminService = inject(AdminService);
  users: User[] = [];
  bsModalRef  = new BsModalRef<RolesModal>();
  private bsModalService = inject(BsModalService);
  // usernames:string[] = [];
  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  openRolesModal(user:User){
    const initialState: ModalOptions = {
      class:'modal-lg',
      initialState : {
        title:  `User Roles`,
        availableRoles:['Admin','Moderator','Member'],
        selectedRoles:[...user.roles],
        username: user.userName,
        rolesUpdate: false
      }
    }

    this.bsModalRef = this.bsModalService.show(RolesModal,initialState);

    this.bsModalRef.onHide?.subscribe({
      next: () => {
        if(this.bsModalRef.content && this.bsModalRef.content.rolesUpdated){
          const selectedRoles = this.bsModalRef.content.selectedRoles;
          this.adminService.updateUserRoles(user.userName,selectedRoles).subscribe({
            next: roles => {
              user.roles = roles;
          }

          }) 
        }
      }
   })
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe({
      next: res => {
        this.users = res;
        // this.users.forEach(u => {
        //   this.usernames.push(u.userName);
        // })  
        
      },
    }
    )};
  
}
