import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { unionBy } from 'lodash';
import { Apollo, QueryRef } from 'apollo-angular';
import gql from 'graphql-tag';
import { ICpuLoadStatus } from '../interfaces/raspberry-pi';
import { Connection } from '../interfaces/connection';
import { DEFAULT_PAGE_SIZE } from '../constants/consts';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class CpuLoadStatusService {

  protected cpuLoadStatuses$: BehaviorSubject<Connection<ICpuLoadStatus>> = new BehaviorSubject({
    items: [],
    totalCount: 0,
    pageInfo: {
      endCursor: null,
      hasNextPage: false,
      startCursor: null,
      hasPreviousPage: false
    }
  });
  searchQuery: QueryRef<any>;
  afterCursor: string = null;
  beforeCursor: string = null;
  searchQueryResult: Observable<Connection<ICpuLoadStatus>>;
  
  constructor(private apollo: Apollo,
    private errorHandlingService: ErrorHandlingService) {
  }

  getFirstCpuLoadStatuses(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<ICpuLoadStatus>> {
    const variables = {
      firstLoadStatuses: pageSize,
      afterLoadStatuses: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ cpuLoadStatuses: Connection<ICpuLoadStatus> }>({
        query: gql`
          query CpuLoadStatuses($firstLoadStatuses: Int, $afterLoadStatuses: String) {
            raspberryPi {
              cpu {
                loadStatuses(first: $firstLoadStatuses, after: $afterLoadStatuses) {
                  items {
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
            const connection = result.data.raspberryPi.cpu.loadStatuses as Connection<ICpuLoadStatus>;
            this.beforeCursor = connection.pageInfo.startCursor;
            this.afterCursor = connection.pageInfo.endCursor;
            return connection;
          }),
          tap(c => this.cpuLoadStatuses$.next(c)),
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
          afterLoadStatuses: this.afterCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.cpu.loadStatuses.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              cpu: {
                loadStatuses: {
                  items: unionBy(prev.raspberryPi.cpu.loadStatuses.items, fetchMoreResult.raspberryPi.cpu.loadStatuses.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.cpu.loadStatuses.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.cpu.loadStatuses.totalCount,
                  __typename: 'CpuLoadStatusConnection'
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

  getLastCpuLoadStatuses(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<ICpuLoadStatus>> {
    const variables = {
      lastLoadStatuses: pageSize,
      beforeLoadStatuses: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ cpuLoadStatuses: Connection<ICpuLoadStatus> }>({
        query: gql`
          query CpuLoadStatuses($lastLoadStatuses: Int, $beforeLoadStatuses: String) {
            raspberryPi {
              cpu {
                loadStatuses(last: $lastLoadStatuses, before: $beforeLoadStatuses) {
                  items {
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
            const connection = result.data.raspberryPi.cpu.loadStatuses as Connection<ICpuLoadStatus>;
            this.beforeCursor = connection.pageInfo.startCursor;
            this.afterCursor = connection.pageInfo.endCursor;
            return connection;
          }),
          tap(c => this.cpuLoadStatuses$.next(c)),
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
          beforeLoadStatuses: this.beforeCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.cpu.loadStatuses.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              cpu: {
                loadStatuses: {
                  items: unionBy(prev.raspberryPi.cpu.loadStatuses.items, fetchMoreResult.raspberryPi.cpu.loadStatuses.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.cpu.loadStatuses.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.cpu.loadStatuses.totalCount,
                  __typename: 'CpuLoadStatusConnection'
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

  subscribeToNewCpuLoadStatuses() {
    this.searchQuery.subscribeToMore({
      document: gql`
      subscription CpuLoadStatus {
        cpuLoadStatus {
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
      }`,
      updateQuery: (prev, { subscriptionData }) => {
        if (!subscriptionData.data) {
          return prev;
        }
        const newCpuLoadStatus = subscriptionData.data.cpuLoadStatus;
        return Object.assign({}, prev, {
          raspberryPi: {
            cpu: {
              loadStatuses: {
                items: [newCpuLoadStatus, ...prev.raspberryPi.cpu.loadStatuses.items],
                pageInfo: prev.raspberryPi.cpu.loadStatuses.pageInfo,
                totalCount: prev.raspberryPi.cpu.loadStatuses.totalCount + 1,
                __typename: 'CpuLoadStatusConnection'
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
