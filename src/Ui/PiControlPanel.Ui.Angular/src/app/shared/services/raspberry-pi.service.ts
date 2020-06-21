import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, finalize } from 'rxjs/operators';
import { get, isNil, orderBy } from 'lodash';
import { Apollo } from 'apollo-angular';
import gql from 'graphql-tag';
import { IRaspberryPi } from '@interfaces/raspberry-pi';
import { ErrorHandlingService } from './error-handling.service';
import { CpuMaxFrequencyLevel } from '@constants/cpu-max-frequency-level';
import { OverlayRef, Overlay } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { MatSpinner } from '@angular/material/progress-spinner';

@Injectable({
  providedIn: 'root',
})
export class RaspberryPiService {
  private overlayRef: OverlayRef;

  constructor(
    private apollo: Apollo,
    private overlay: Overlay,
    private errorHandlingService: ErrorHandlingService
  ) {
    this.overlayRef = this.createOverlay();
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
              scalingGovernor
              maxFrequency
              frequency {
                value
                dateTime
              }
              sensorsStatus {
                temperature
                voltage
                underVoltageDetected
                armFrequencyCapped
                currentlyThrottled
                softTemperatureLimitActive
                underVoltageOccurred
                armFrequencyCappingOccurred
                throttlingOccurred
                softTemperatureLimitOccurred
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
              fileSystems {
                name
                total
                type
                status {
                  fileSystemName
                  available
                  used
                  dateTime
                }
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
              frequency
            }
            os {
              name
              kernel
              hostname
              upgradeablePackages
              status {
                uptime
                dateTime
              }
            }
            network {
              networkInterfaces {
                name
                ipAddress
                subnetMask
                defaultGateway
                status {
                  networkInterfaceName
                  totalReceived
                  receiveSpeed
                  totalSent
                  sendSpeed
                  dateTime
                }
              }
            }
          }
        }`,
      fetchPolicy: 'network-only'
    }).pipe(
      map(result => {
        const raspberryPi = get(result.data, 'raspberryPi');
        const processes = get(raspberryPi, 'cpu.loadStatus.processes');
        if (!isNil(processes)) {
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
    this.showSpinner();
    return this.apollo.mutate({
      mutation: gql`	
        mutation update {	
          update	
        }`
    }).pipe(
      map(result => get(result.data, 'update')),
      catchError(this.errorHandlingService.handleError),
      finalize(() => this.stopSpinner())
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

  overclockRaspberryPi(cpuMaxFrequencyLevel: CpuMaxFrequencyLevel): Observable<boolean> {
    return this.apollo.mutate({
      mutation: gql`
        mutation overclock($cpuMaxFrequencyLevel: CpuMaxFrequencyLevel!) {
          overclock(cpuMaxFrequencyLevel: $cpuMaxFrequencyLevel)
        }`,
      variables: {
        cpuMaxFrequencyLevel: cpuMaxFrequencyLevel
      }
    }).pipe(
      map(result => get(result.data, 'overclock')),
      catchError(this.errorHandlingService.handleError)
    );
  }

  private createOverlay() {
    return this.overlay.create({
      hasBackdrop: true,
      backdropClass: 'dark-backdrop',
      positionStrategy: this.overlay.position()
        .global()
        .centerHorizontally()
        .centerVertically()
    });
  }

  private showSpinner() {
    this.overlayRef.attach(new ComponentPortal(MatSpinner));
  }

  private stopSpinner() {
    if (this.overlayRef.hasAttached()) {
      this.overlayRef.detach();
    }
  }

}
