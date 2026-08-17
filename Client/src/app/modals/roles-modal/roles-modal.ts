import { Component, inject } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  imports: [],
  templateUrl: './roles-modal.html',
  styleUrl: './roles-modal.css',
})
export class RolesModal {
  bsModalRef = inject(BsModalRef);
  title = '';
  username = '';
  availableRoles: string[] = [];
  selectedRoles: string[] = [];
  rolesUpdated  = false;
  
  onSelectRoles(){
    this.rolesUpdated = true;
    // this.username = username;
    // this.selectedRoles = selectedRoles;
    this.bsModalRef.hide();
  }


  updateSelectedRole(selectedRole:string) {
    if(this.selectedRoles.includes(selectedRole)){
      this.selectedRoles = this.selectedRoles.filter(r => r!== selectedRole);
    }else{
      this.selectedRoles.push(selectedRole);
    }
  }
}
