import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { Register } from '../register/register';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  imports: [RouterLink, RouterOutlet, Register],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  registerMode = false;
  http = inject(HttpClient);
  users: any;

  registerModeToggle() {
    this.registerMode = !this.registerMode;
  }

  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }

  ngOnInit(): void {
    this.getUsers();
  }

  getUsers() {
    this.http.get('https://localhost:5700/api/users').subscribe({
      next: (response) => {
        this.users = response;
      },
      error: (error) => {
        console.log(error);
      },
      complete: () => {
        console.log('Request has completed.');
      },
    });
  }
}
