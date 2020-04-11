import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { get, isNil, orderBy } from 'lodash';
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
              frequency {
                value
                dateTime
              }
              temperature {
                value
                dateTime
              }
              loadStatus {
                dateTime
                lastMinuteAverage
                last5MinutesAverage
                last15MinutesAverage
                kernelRealTime
                userRealTime
                totalRealTime
                processes {
                  processId
                  user
                  priority
                  niceValue
                  totalMemory
                  ram
                  sharedMemory
                  state
                  cpuPercentage
                  ramPercentage
                  totalCpuTime
                  command
                }
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
      map(result => {
        var raspberryPi = get(result.data, 'raspberryPi');
        var processes = get(raspberryPi, 'cpu.loadStatus.processes');
        if(!isNil(processes)){
          raspberryPi.cpu.loadStatus.processes = orderBy(
            processes,
            ['cpuPercentage', 'ramPercentage'],
            ['desc', 'desc']);
        }
        return raspberryPi;
      }),
      catchError(this.errorHandlingService.handleError)
    );
  }

  rebootRaspberryPi(): Observable<boolean> {
    return this.apollo.mutate({
      mutation: gql`
        mutation reboot {
          reboot
        }`
    }).pipe(
      map(result => get(result.data, 'reboot')),
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

  updateRaspberryPi(): Observable<boolean> {
    return this.apollo.mutate({
      mutation: gql`
        mutation update {
          update
        }`
    }).pipe(
      map(result => get(result.data, 'update')),
      catchError(this.errorHandlingService.handleError)
    );
  }

  killProcess(processId: number): Observable<boolean> {
    return this.apollo.mutate({
      mutation: gql`
        mutation kill($processId: Int!) {
          kill(processId: $processId)
        }`,
      variables: {
        processId: processId
      }
    }).pipe(
      map(result => get(result.data, 'kill')),
      catchError(this.errorHandlingService.handleError)
    );
  }

}
