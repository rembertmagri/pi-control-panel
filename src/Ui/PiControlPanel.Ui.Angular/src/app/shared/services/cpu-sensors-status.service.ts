import { Injectable } from '@angular/core';
import { Observable, timer } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { unionBy, isNil } from 'lodash';
import { Apollo, QueryRef } from 'apollo-angular';
import gql from 'graphql-tag';
import { ICpuSensorsStatus } from '@interfaces/raspberry-pi';
import { Connection } from '@interfaces/connection';
import { DEFAULT_PAGE_SIZE, QUERY_REFETCH_DUE_TIME, QUERY_REFETCH_PERIOD } from '@constants/consts';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class CpuSensorsStatusService {
  searchQuery: QueryRef<any>;
  afterCursor: string = null;
  beforeCursor: string = null;
  searchQueryResult: Observable<Connection<ICpuSensorsStatus>>;
  
  constructor(private apollo: Apollo,
    private errorHandlingService: ErrorHandlingService) {
  }

  getFirstCpuSensorsStatuses(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<ICpuSensorsStatus>> {
    const variables = {
      firstSensorsStatuses: pageSize,
      afterSensorsStatuses: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ cpuSensorsStatuses: Connection<ICpuSensorsStatus> }>({
        query: gql`
          query CpuSensorsStatuses($firstSensorsStatuses: Int, $afterSensorsStatuses: String) {
            raspberryPi {
              cpu {
                sensorsStatuses(first: $firstSensorsStatuses, after: $afterSensorsStatuses) {
                items {
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
                  pageInfo {
                    startCursor
                    hasPreviousPage
                    endCursor
                    hasNextPage
                  }
                  totalCount
                }
              }
            }
          }`,
        variables,
        fetchPolicy: 'network-only'
      });
      this.searchQueryResult = this.searchQuery.valueChanges
        .pipe(
          map(result => {
            const connection = result.data.raspberryPi.cpu.sensorsStatuses as Connection<ICpuSensorsStatus>;
            this.beforeCursor = connection.pageInfo.startCursor;
            this.afterCursor = connection.pageInfo.endCursor;
            return connection;
          }),
          catchError((err) => this.errorHandlingService.handleError(err))
        );
    } else {
      this.searchQuery.setVariables(variables, true, true);
    }
    return this.searchQueryResult;
  }

  getNextPage() {
    if (this.searchQuery) {
      this.searchQuery.fetchMore({
        variables: {
          afterSensorsStatuses: this.afterCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.cpu.sensorsStatuses.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              cpu: {
                sensorsStatuses: {
                  items: unionBy(prev.raspberryPi.cpu.sensorsStatuses.items, fetchMoreResult.raspberryPi.cpu.sensorsStatuses.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.cpu.sensorsStatuses.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.cpu.sensorsStatuses.totalCount,
                  __typename: 'CpuSensorsStatusConnection'
                },
                __typename: 'CpuType'
              },
              __typename: 'RaspberryPiType'
            }
          });
        })
      });
    }
  }

  getLastCpuSensorsStatuses(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<ICpuSensorsStatus>> {
    const variables = {
      lastSensorsStatuses: pageSize,
      beforeSensorsStatuses: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ cpuSensorsStatuses: Connection<ICpuSensorsStatus> }>({
        query: gql`
          query CpuSensorsStatuses($lastSensorsStatuses: Int, $beforeSensorsStatuses: String) {
            raspberryPi {
              cpu {
                sensorsStatuses(last: $lastSensorsStatuses, before: $beforeSensorsStatuses) {
                items {
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
                  pageInfo {
                    startCursor
                    hasPreviousPage
                    endCursor
                    hasNextPage
                  }
                  totalCount
                }
              }
            }
          }`,
        variables,
        fetchPolicy: 'network-only'
      });
      this.searchQueryResult = this.searchQuery.valueChanges
        .pipe(
          map(result => {
            const connection = result.data.raspberryPi.cpu.sensorsStatuses as Connection<ICpuSensorsStatus>;
            this.beforeCursor = connection.pageInfo.startCursor;
            this.afterCursor = connection.pageInfo.endCursor;
            return connection;
          }),
          catchError((err) => this.errorHandlingService.handleError(err))
        );
    } else {
      this.searchQuery.setVariables(variables, true, true);
    }
    return this.searchQueryResult;
  }

  getPreviousPage() {
    if (this.searchQuery) {
      this.searchQuery.fetchMore({
        variables: {
          beforeSensorsStatuses: this.beforeCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.cpu.sensorsStatuses.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              cpu: {
                sensorsStatuses: {
                  items: unionBy(prev.raspberryPi.cpu.sensorsStatuses.items, fetchMoreResult.raspberryPi.cpu.sensorsStatuses.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.cpu.sensorsStatuses.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.cpu.sensorsStatuses.totalCount,
                  __typename: 'CpuSensorsStatusConnection'
                },
                __typename: 'CpuType'
              },
              __typename: 'RaspberryPiType'
            }
          });
        })
      });
    }
  }

  subscribeToNewCpuSensorsStatuses() {
    if (this.searchQuery) {
      this.searchQuery.subscribeToMore({
        document: gql`
        subscription CpuSensorsStatus {
          cpuSensorsStatus {
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
        }`,
        updateQuery: (prev, { subscriptionData }) => {
          if (!subscriptionData.data) {
            return prev;
          }
          const newCpuSensorsStatus = subscriptionData.data.cpuSensorsStatus;
          return Object.assign({}, prev, {
            raspberryPi: {
              cpu: {
                sensorsStatuses: {
                  items: [newCpuSensorsStatus, ...prev.raspberryPi.cpu.sensorsStatuses.items],
                  pageInfo: prev.raspberryPi.cpu.sensorsStatuses.pageInfo,
                  totalCount: prev.raspberryPi.cpu.sensorsStatuses.totalCount + 1,
                  __typename: 'CpuSensorsStatusConnection'
                },
                __typename: 'CpuType'
              },
              __typename: 'RaspberryPiType'
            }
          });
        }
      });
    }
  }

  refetchPeriodically(): Observable<boolean> {
    return timer(QUERY_REFETCH_DUE_TIME, QUERY_REFETCH_PERIOD)
      .pipe(
        tap(() => {
          if (this.searchQuery) {
            this.searchQuery.resetLastResults();
            this.searchQuery.refetch();
          }
        }),
        map(result => !isNil(result))
      );
  }

  refetch() {
    if (this.searchQuery) {
      this.searchQuery.refetch();
    }
  }

}
