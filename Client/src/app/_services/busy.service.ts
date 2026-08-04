import { inject, Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  spinnerService = inject(NgxSpinnerService);

  busyRequestCount = 0;

  busy() {
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      fullScreen: true,
      type: 'line-spin-fade',
      color: 'rgb(252,150,245)',
      // bdColor:'rgb(177, 204, 243)'
      // bdColor:'rgb(79, 81, 84)'
      // bdColor:'rgb(129, 131, 135)'
      bdColor: 'rgba(10, 11, 11, 0.91)',
    });
  }

  idle() {
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0) this.busyRequestCount = 0;
    this.spinnerService.hide();
  }
}
