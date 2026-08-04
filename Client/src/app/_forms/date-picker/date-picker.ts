import { NgIf } from '@angular/common';
import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';
import {BsDatepickerConfig, BsDatepickerModule} from 'ngx-bootstrap/datepicker'

@Component({
  selector: 'app-date-picker',
  imports: [BsDatepickerModule,ReactiveFormsModule,NgIf],
  templateUrl: './date-picker.html',
  styleUrl: './date-picker.css',
})
export class DatePicker implements ControlValueAccessor {

  label = input<string>();
  maxDate = input<Date>();
  bsConfig?:Partial<BsDatepickerConfig>;

  constructor(@Self() public ngControl:NgControl){
      this.ngControl.valueAccessor = this;
      this.bsConfig = {
        containerClass:'theme-red',
        dateInputFormat : "DD MMMM YYYY"
      }
  }
  
  writeValue(obj: any): void {
    throw new Error('Method not implemented.');
  }
  registerOnChange(fn: any): void {
    throw new Error('Method not implemented.');
  }
  registerOnTouched(fn: any): void {
    throw new Error('Method not implemented.');
  }
  
  get control():FormControl{
    return this.ngControl.control as FormControl;
  }
  
}
