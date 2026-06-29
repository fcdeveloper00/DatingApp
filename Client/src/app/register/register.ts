import { Component, EventEmitter, inject, input, Input, output, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  model:any = {};
  accountService= inject(AccountService);
  // @Output() cancelRegister = new EventEmitter();
  cancelRegister = output<boolean>();
  register(){
    console.log(this.model);
    
    this.accountService.register(this.model).subscribe({
      next: res =>{
        console.log(res);
        this.cancel();
      }, 
      error: error => console.error(error),
      complete:() => console.log("Successfull Registration!")
    });
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}
