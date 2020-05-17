import { retryWhen, mergeMap, finalize } from 'rxjs/operators';
import { HttpEvent } from '@angular/common/http';
import { Observable, throwError, timer } from 'rxjs';

/**
 * Defines the delayed retry operator based on rxjs retryWhen, using a strategy based on the max number
 * of attempts, the delay between the attempts and the error codes for which we don't want to retry.
 *
 * @param config - an object containing the strategy configuration
 * @param config.maxRetryAttempts - the limit of retries
 * @param config.duration - the delay between consecutive retries
 * @param config.excludedStatusCodes - the error codes for which not to retry
 * @returns the delayed retry operator
 */
export const delayedRetry = ({
  maxRetryAttempts = 2,
  duration = 250,
  excludedStatusCodes = []
}: {
  maxRetryAttempts?: number,
  duration?: number,
  excludedStatusCodes?: number[]
} = {}) => retryWhen<HttpEvent<any>>((attempts: Observable<any>) => {
  return attempts.pipe(
    mergeMap((error, i) => {
      const retryAttempt = i + 1;
      if (
        retryAttempt > maxRetryAttempts ||
        excludedStatusCodes.find(e => e === error.status)
      ) {
        return throwError(error);
      }
      console.error(`Attempt ${retryAttempt}: retrying in ${duration}ms`);
      return timer(duration); // retry after 250ms
    }),
    finalize(() => console.error(`Could not process request after ${maxRetryAttempts} attempts`))
  );
});
