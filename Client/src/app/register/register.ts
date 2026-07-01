import { Component, EventEmitter, inject, input, Input, output, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

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
  toastr = inject(ToastrService);
  cancelRegister = output<boolean>();
  register(){
    console.log(this.model);
    
    this.accountService.register(this.model).subscribe({
      next: res =>{
        console.log(res);
        this.toastr.success('You\'ve been Registered Successfully!')
        this.cancel();
      }, 
      error: error => {
        this.toastr.error(error.error.title)
        console.log(error)
      },
      complete:() => this.toastr.success("Successful operation.")
    });
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}
