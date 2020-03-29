import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { get } from 'lodash';
import { Apollo } from 'apollo-angular';
import gql from 'graphql-tag';
import { IRaspberryPi } from '../interfaces/raspberry-pi';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class RaspberryPiService {

  constructor(private apollo: Apollo,
    private errorHandlingService: ErrorHandlingService) {
  }

  getRaspberryPi(): Observable<IRaspberryPi> {
    return this.apollo.query<{ raspberryPi: IRaspberryPi }>({
      query: gql`
        query RaspberryPi {
          raspberryPi {
            chipset {
              model
              revision
              serial
              version
            }
            cpu {
              cores
              model
              temperature {
                value
                dateTime
              }
              averageLoad {
                lastMinute
                last5Minutes
                last15Minutes
                dateTime
              }
              realTimeLoad {
                kernel
                user
                total
                dateTime
              }
            }
            disk {
              fileSystem
              type
              total
              status {
                used
                available
                dateTime
              }
            }
            ram {
              total
              status {
                used
                free
                diskCache
                dateTime
              }
            }
            swapMemory {
              total
              status {
                used
                free
                dateTime
              }
            }
            gpu {
              memory
            }
            os {
              name
              kernel
              hostname
              status {
                uptime
                dateTime
              }
            }
          }
        }`,
      fetchPolicy: 'network-only'
    }).pipe(
      map(result => get(result.data, 'raspberryPi')),
      catchError(this.errorHandlingService.handleError)
    );
  }

  shutdownRaspberryPi(): Observable<boolean> {
    return this.apollo.mutate({
      mutation: gql`
        mutation shutdown {
          shutdown
        }`
    }).pipe(
      map(result => get(result.data, 'shutdown')),
      catchError(this.errorHandlingService.handleError)
    );
  }

}
