import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { checkGraphQLErrors } from '../operators/check-graphql-errors.operator';
import { get } from 'lodash';
import { UNAUTHORIZED, FORBIDDEN } from 'http-status-codes';
import { delayedRetry } from '../operators/delayed-retry.operator';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService,
    private router: Router) { }

  /*
   * Function to intercept http requests and add an access token if we have one
   * @param {HttpRequest<any>} - the http request
   * @param {HttpHandler} - the next http request handler in the sequence
   * @returns {Observable<HttpEvent<any>>} - an observable of the result of the next handler
   */
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // If not login operation, perform the request as is
    if (get(request.body, 'operationName') === 'login') {
      return next.handle(request);
    }
    const token = localStorage.getItem('jwt');
    return this.handleRequest(request, next, token);
  }

  handleRequest(request: HttpRequest<any>, next: HttpHandler, token: string): Observable<HttpEvent<any>> {
    // add the token to the request headers(req is readonly so clone it)
    request = request.clone({ headers: request.headers.set('Authorization', `Bearer ${token}`) });
    return next.handle(request)
      .pipe(
        checkGraphQLErrors(),
        delayedRetry({ excludedStatusCodes: [UNAUTHORIZED, FORBIDDEN] }), // don't retry if unauthorized or token expired
        catchError(error => {
          console.error(`Error '${error.name}' while handling request.`);
          if (error.status === UNAUTHORIZED) {
            alert('Your session has expired, please login again');
            this.authService.logout();
            this.router.navigate(['/login']);
          }
          return next.handle(request);
        })
      );
  }

}
