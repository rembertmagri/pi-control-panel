import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap, map } from 'rxjs/operators';
import { isNil, isEmpty } from 'lodash';
import { LoginService } from './login.service';
import { IUserAccount } from '../interfaces/userAccount';
import { ROLE } from '../constants/role';

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

  isLoggedIn() {
    const roles = this.getRoles();
    return !isNil(roles) && roles.includes(ROLE.USER);
  }

  isSuperUser() {
    var roles = this.getRoles();
    if (isNil(roles) || !roles.includes(ROLE.USER)) {
      return false;
    }
    return roles.includes(ROLE.SUPER_USER);
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
    return JSON.parse(rolesFromStorage) as ROLE[];
  }

}
