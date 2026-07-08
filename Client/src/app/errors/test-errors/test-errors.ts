import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-test-errors',
  imports: [],
  templateUrl: './test-errors.html',
  styleUrl: './test-errors.css',
})
export class TestErrors {

  baseUrl = environment.apiUrl;
  private httpClient = inject(HttpClient);
  validationErrors:string[] = [];

  get400Error(){
    this.httpClient.get(`${this.baseUrl}/buggy/bad-request`).subscribe({
      next:(response) => console.log(response),
      error: error => console.error(error),
      complete: () => console.log('Request completed.')
    });
  }

  get401Error(){
    this.httpClient.get(`${this.baseUrl}/buggy/auth`).subscribe({
      next:response => console.log(response),
      error:error => console.error(error),
      complete: () => console.log('Request completed.')
    })
  }

   get404Error(){
    this.httpClient.get(`${this.baseUrl}/buggy/not-found`).subscribe({
      next:response => console.log(response),
      error:error => console.error(error),
      complete: () => console.log('Request completed.')
    })
  }

  get500Error(){
    this.httpClient.get(`${this.baseUrl}/buggy/server-error`).subscribe({
      next:response => console.log(response),
      error:error => console.error(error),
      complete: () => console.log('Request completed.')
    })
  }

  get400ValidationError(){
    this.httpClient.post(`${this.baseUrl}/account/register`,{}).subscribe({
      next:response => console.log(response),
      error:error => {
        // console.error(error),
        this.validationErrors = error;
      },
      complete: () => console.log('Request completed.')
    })
  }
}
