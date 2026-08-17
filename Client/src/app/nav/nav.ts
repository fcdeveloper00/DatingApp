import { Component, inject, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
 import {BsDropdownModule}from 'ngx-bootstrap/dropdown'
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { ToastrService } from 'ngx-toastr';
import { HasRoleDirective } from "../_directives/has-role.directive";
@Component({
  selector: 'app-nav',
  imports: [FormsModule, BsDropdownModule, RouterLink, RouterLinkActive, RouterOutlet, HasRoleDirective],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  public model: any = {};
  public accountService = inject(AccountService);
  private toastr = inject(ToastrService);
  router = inject(Router);
  // loggedIn = false;

  login() {
    this.accountService.login(this.model).subscribe({
      next: _ => {
        // console.log(response);
        this.router.navigateByUrl('/members');
        // let username :string = this.model.username;
        // username = username.replace(this.model.username[0].toString(),this.model.username[0].toString().toUppsercase());
        this.toastr.success(`Welcome ${this.model.username}!`);
      },
      error: (error) => this.toastr.error(`${error.error} - Status: ${error.status}`)
    
      // error: (error) => console.log(error),
    });
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/')
  }
}

/* import { Directive, inject, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]',
})
export class HasRole implements OnInit{
  @Input() appHasRole:string[] = [];
  private accountService = inject(AccountService);
  private viewContainerRef  = inject(ViewContainerRef);
  private templateRef  = inject(TemplateRef);

  ngOnInit(){
    if(this.accountService.roles().some(r => this.appHasRole.includes(r))){
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    }else{
      this.viewContainerRef.clear();
    }
  }


}
 */