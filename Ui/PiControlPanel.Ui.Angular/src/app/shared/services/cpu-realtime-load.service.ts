import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { unionBy } from 'lodash';
import { Apollo, QueryRef } from 'apollo-angular';
import gql from 'graphql-tag';
import { ICpuRealTimeLoad } from '../interfaces/raspberry-pi';
import { Connection } from '../interfaces/connection';
import { DEFAULT_PAGE_SIZE } from '../constants/consts';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class CpuRealTimeLoadService {

  protected cpuRealTimeLoads$: BehaviorSubject<Connection<ICpuRealTimeLoad>> = new BehaviorSubject({
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
  searchQueryResult: Observable<Connection<ICpuRealTimeLoad>>;
  
  constructor(private apollo: Apollo,
    private errorHandlingService: ErrorHandlingService) {
  }

  getFirstCpuRealTimeLoads(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<ICpuRealTimeLoad>> {
    const variables = {
      firstRealTimeLoads: pageSize,
      afterRealTimeLoads: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ cpuRealTimeLoads: Connection<ICpuRealTimeLoad> }>({
        query: gql`
          query CpuRealTimeLoads($firstRealTimeLoads: Int, $afterRealTimeLoads: String) {
            raspberryPi {
              cpu {
                realTimeLoads(first: $firstRealTimeLoads, after: $afterRealTimeLoads) {
                  items {
                    kernel
                    user
                    total
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
            const connection = result.data.raspberryPi.cpu.realTimeLoads as Connection<ICpuRealTimeLoad>;
            this.beforeCursor = connection.pageInfo.startCursor;
            this.afterCursor = connection.pageInfo.endCursor;
            return connection;
          }),
          tap(c => this.cpuRealTimeLoads$.next(c)),
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
          afterRealTimeLoads: this.afterCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.cpu.realTimeLoads.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              cpu: {
                realTimeLoads: {
                  items: unionBy(prev.raspberryPi.cpu.realTimeLoads.items, fetchMoreResult.raspberryPi.cpu.realTimeLoads.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.cpu.realTimeLoads.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.cpu.realTimeLoads.totalCount,
                  __typename: 'CpuRealTimeLoadConnection'
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

  getLastCpuRealTimeLoads(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<ICpuRealTimeLoad>> {
    const variables = {
      lastRealTimeLoads: pageSize,
      beforeRealTimeLoads: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ cpuRealTimeLoads: Connection<ICpuRealTimeLoad> }>({
        query: gql`
          query CpuRealTimeLoads($lastRealTimeLoads: Int, $beforeRealTimeLoads: String) {
            raspberryPi {
              cpu {
                realTimeLoads(last: $lastRealTimeLoads, before: $beforeRealTimeLoads) {
                  items {
                    kernel
                    user
                    total
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
            const connection = result.data.raspberryPi.cpu.realTimeLoads as Connection<ICpuRealTimeLoad>;
            this.beforeCursor = connection.pageInfo.startCursor;
            this.afterCursor = connection.pageInfo.endCursor;
            return connection;
          }),
          tap(c => this.cpuRealTimeLoads$.next(c)),
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
          beforeRealTimeLoads: this.beforeCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.cpu.realTimeLoads.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              cpu: {
                realTimeLoads: {
                  items: unionBy(prev.raspberryPi.cpu.realTimeLoads.items, fetchMoreResult.raspberryPi.cpu.realTimeLoads.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.cpu.realTimeLoads.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.cpu.realTimeLoads.totalCount,
                  __typename: 'CpuRealTimeLoadConnection'
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

  subscribeToNewCpuRealTimeLoads() {
    this.searchQuery.subscribeToMore({
      document: gql`
      subscription CpuRealTimeLoad {
        cpuRealTimeLoad {
          kernel
          user
          total
          dateTime
        }
      }`,
      updateQuery: (prev, { subscriptionData }) => {
        if (!subscriptionData.data) {
          return prev;
        }
        const newCpuRealTimeLoad = subscriptionData.data.cpuRealTimeLoad;
        return Object.assign({}, prev, {
          raspberryPi: {
            cpu: {
              realTimeLoads: {
                items: [newCpuRealTimeLoad, ...prev.raspberryPi.cpu.realTimeLoads.items],
                pageInfo: prev.raspberryPi.cpu.realTimeLoads.pageInfo,
                totalCount: prev.raspberryPi.cpu.realTimeLoads.totalCount + 1,
                __typename: 'CpuRealTimeLoadConnection'
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
