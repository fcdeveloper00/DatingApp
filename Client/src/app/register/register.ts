import {  Component,  EventEmitter,  inject,  input,  Input,  OnInit,  output,  Output,  Self,} from '@angular/core';
import {  AbstractControl,  FormBuilder,  FormControl,  FormGroup,  ReactiveFormsModule,  ValidatorFn,  Validators,} from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { JsonPipe, NgIf } from '@angular/common';
import { TextInput } from '../_forms/text-input/text-input';
import { DatePicker } from '../_forms/date-picker/date-picker';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, JsonPipe, NgIf, TextInput, DatePicker],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register implements OnInit {
  // @Output() cancelRegister = new EventEmitter();
  model: any = {};

  toastr = inject(ToastrService);

  cancelRegister = output<boolean>();

  private accountService = inject(AccountService);

  private formBuilder = inject(FormBuilder);

  private router = inject(Router);

  //Tracks the value and validity state of a group of 'FormControl' instances
  registerForm: FormGroup = new FormGroup({});

  maxDate = new Date();

  validationErrors: string[] | undefined;

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    //Here, We initialize our 'FormControl' s
    // What inputs are we going to support inside our form.
    this.registerForm = this.formBuilder.group({
      // The first parameter inside our 'FormControl' is the initial value of the input
      username: ['', Validators.required],

      //If we want to have more than one validator for a single input, then we can use an array to specify our validators
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],

      confirmPassword: ['', [Validators.required, this.matchValues('password')]],

      knownAs: ['', Validators.required],

      dateOfBirth: ['', Validators.required],

      city: ['', Validators.required],

      country: ['', Validators.required],

      gender: ['male', Validators.required],
    });

    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity(),
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value
        ? null
        : {
            isMatching: true,
          }; /* We give our validator a name (It can be anything). True means these Two controls do not match AND
      This is effectively the name of the validator we're comparing against in the templates.*/
    };
  }

  register() {
    // console.log(this.registerForm.value);
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
    this.registerForm.patchValue({ dateOfBirth: dob });
    this.accountService.register(this.registerForm.value).subscribe({
      next: (_) => {
        this.router.navigateByUrl('/members');
        this.toastr.success("You've been Registered Successfully!");
      },
      error: (error) => {
        this.validationErrors = error;
        console.log(error);
      },
      complete: () => this.toastr.success('Successful operation.'),
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;
    return new Date(dob).toISOString().slice(0, 10);
  }
}
