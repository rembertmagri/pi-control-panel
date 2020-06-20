import { Injectable } from '@angular/core';
import { throwError } from 'rxjs';
import { isNil } from 'lodash';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ErrorHandlingService {

  handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `message was: ${error.message}`);
      if (error.status === 401) { //unauthorized
        return throwError('Unauthorized; please login again.');
      }
      if (!isNil(error.error.responseType)) {
        switch (error.error.responseType) {
          case 2: // Warning
          case 3: // Error
            alert(error.error.message);
            break;
        }
      }
    }
    // return an observable with a user-facing error message
    return throwError('Something bad happened; please try again later.');
  }

}
