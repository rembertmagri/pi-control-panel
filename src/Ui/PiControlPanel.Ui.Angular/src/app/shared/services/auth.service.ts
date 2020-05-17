import { Injectable } from '@angular/core';
import { Observable, timer } from 'rxjs';
import { tap, map, switchMap } from 'rxjs/operators';
import { isNil, isEmpty } from 'lodash';
import { LoginService } from './login.service';
import { IUserAccount } from '../interfaces/userAccount';
import { Role } from '../constants/role';
import { TOKEN_REFRESH_DUE_TIME, TOKEN_REFRESH_PERIOD } from '../constants/consts';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  constructor(protected loginService: LoginService) { }

  login(userAccount: IUserAccount): Observable<boolean> {
    return this.loginService.login(userAccount)
      .pipe(
        tap((result) => {
          localStorage.setItem('jwt', result.jwt);
          localStorage.setItem('roles', JSON.stringify(result.roles));
          localStorage.setItem('username', result.username);
        }),
        map(result => !isNil(result))
      );
  }

  refreshTokenPeriodically(): Observable<boolean> {
    return timer(TOKEN_REFRESH_DUE_TIME, TOKEN_REFRESH_PERIOD)
      .pipe(
        switchMap(() => this.loginService.refreshToken()),
        tap((result) => {
          localStorage.setItem('jwt', result.jwt);
          localStorage.setItem('roles', JSON.stringify(result.roles));
          localStorage.setItem('username', result.username);
        }),
        map(result => !isNil(result))
      );
  }

  isLoggedIn() {
    const roles = this.getRoles();
    return !isNil(roles) && roles.includes(Role.USER);
  }

  isSuperUser() {
    var roles = this.getRoles();
    if (isNil(roles) || !roles.includes(Role.USER)) {
      return false;
    }
    return roles.includes(Role.SUPER_USER);
  }

  getLoggedInUsername() {
    return localStorage.getItem('username');
  }

  logout() {
    localStorage.removeItem('jwt');
    localStorage.removeItem('roles');
    localStorage.removeItem('username');
  }

  private getRoles() {
    const rolesFromStorage = localStorage.getItem('roles');
    if (isEmpty(rolesFromStorage)) {
      return null;
    }
    return JSON.parse(rolesFromStorage) as Role[];
  }

}
