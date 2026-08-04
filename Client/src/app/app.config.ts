import { ApplicationConfig, importProvidersFrom, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import {provideAnimations} from '@angular/platform-browser/animations'
import { routes } from './app.routes';
import { provideToastr } from 'ngx-toastr';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { errorInterceptor } from './_interceptors/error-interceptor';
import { jwtInterceptor } from './_interceptors/jwt-interceptor';
import {NgxSpinnerModule} from 'ngx-spinner';
import { loadingInterceptor } from './_interceptors/loading-interceptor';
import{TimeagoModule} from 'ngx-timeago';
import { NgForm } from '@angular/forms';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([errorInterceptor,jwtInterceptor,loadingInterceptor])),
    provideAnimations(),
    provideToastr({
      positionClass:'toast-bottom-right',
      timeOut:4000,
      preventDuplicates:true,
    }),
    importProvidersFrom(NgxSpinnerModule,TimeagoModule.forRoot())
  ]
};
