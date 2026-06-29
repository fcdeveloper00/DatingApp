import { Component, inject, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
 import {BsDropdownModule}from 'ngx-bootstrap/dropdown'
@Component({
  selector: 'app-nav',
  imports: [FormsModule,BsDropdownModule],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  public model: any = {};
  public accountService = inject(AccountService);
  // loggedIn = false;

  login() {
    this.accountService.login(this.model).subscribe({
      next: (response) => {
        console.log(response);
        // this.loggedIn = true;
      },
      error: (error) => console.error(error),
    });
  }

  logout(){
    this.accountService.logout();
    // this.loggedIn = false;
  }
}
