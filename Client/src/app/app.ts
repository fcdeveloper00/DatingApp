import { Component, inject, Input, OnInit, signal } from '@angular/core';
import {FormsModule} from '@angular/forms'
import { RouterOutlet } from '@angular/router';
import { Nav } from "./nav/nav";
import { AccountService } from './_services/account.service';
import { User } from './_models/user.model';
import { Home } from "./home/home";
import { NgxSpinnerComponent } from 'ngx-spinner';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FormsModule, Nav, Home,NgxSpinnerComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  protected readonly title = signal('Client');
  accountService = inject(AccountService);
  

  ngOnInit(): void {
    console.log('AppComponent is instantiated');
    this.setCurrentUser();
  }

  setCurrentUser(){
    const storageValue = localStorage.getItem('user');
    if(storageValue === null) return;
    const user = JSON.parse(storageValue);
    this.accountService.setCurrentUser(user);
  }

  
}
