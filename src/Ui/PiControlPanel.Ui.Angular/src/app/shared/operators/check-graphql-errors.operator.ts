import { map } from 'rxjs/operators';
import { some, isNil, get } from 'lodash';
import { HttpEvent, HttpEventType, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { UNAUTHORIZED } from 'http-status-codes';

/**
 * Checks if an error object corresponds to a GraphQL Authorization error based on the error code
 *
 * @param error - the error object to be tested
 * @returns whether the error is a GraphQL Authorization error
 */
const isAuthorizationError = (error: any) => {
  const code = get(error, 'extensions.code');
  return !isNil(code) && code === 'authorization';
};

/**
 * Contains the logic to check for GraphQL authorization errors in the HTTP response body.
 * If the request payload contains the authorization error, throws an Unauthorized HTTP response;
 * if not, just passes on the original request response.
 *
 * @returns the operator containing the original request response
 * @throws an Unauthorized HTTP response in case a GraphQL error is present in the response body.
 */
export const checkGraphQLErrors = () => map<HttpEvent<any>, HttpEvent<any>>(val => {
  if (val.type === HttpEventType.Response) {
    const response = val as HttpResponse<any>;
    const errors = get(response, 'body.errors');
    if (!isNil(errors) && some(errors, (error) => isAuthorizationError(error))) {
      throw new HttpErrorResponse({
        headers: response.headers,
        status: UNAUTHORIZED,
        statusText: response.statusText,
        url: response.url
      });
    }
  }
  return val;
});
