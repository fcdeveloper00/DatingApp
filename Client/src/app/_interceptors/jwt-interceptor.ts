import { HttpHeaders, HttpInterceptorFn } from '@angular/common/http';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';
import { environment } from '../../environments/environment';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(AccountService).currentUser()?.token ;
  // const authUrl = environment.apiUrl;
  // req = req.clone({
  //   headers: new HttpHeaders({
  //     Authorization:`Bearer ${accountService.currentUser()?.token}`
  //   })
  // })

  if(token){
    // const wreq = req.clone({
    //   headers: req.headers.append('Authorization',token)
    // })
    
    req = req.clone({
      setHeaders:{
        Authorization: `Bearer ${token}`,
        // Url:`${authUrl}/users`
      }
    });
  }  
  

  return next(req);
};
