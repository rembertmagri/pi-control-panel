import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap, map } from 'rxjs/operators';
import { isNil, isEmpty } from 'lodash';
import { LoginService } from './login.service';
import { IUserAccount } from '../interfaces/userAccount';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  constructor(protected loginService: LoginService) { }

  login(userAccount: IUserAccount): Observable<boolean> {
    return this.loginService.login(userAccount)
      .pipe(
        tap((result) => {
          localStorage.setItem('jwt_token', result.jwt);
        }),
        map(result => !isNil(result))
      );
  }

  isLoggedIn() {
    return !isEmpty(localStorage.getItem('jwt_token'));
  }

  logout() {
    localStorage.removeItem('jwt_token');
  }

}
