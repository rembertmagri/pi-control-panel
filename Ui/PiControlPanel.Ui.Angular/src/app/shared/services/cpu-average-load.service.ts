import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { unionBy } from 'lodash';
import { Apollo, QueryRef } from 'apollo-angular';
import gql from 'graphql-tag';
import { ICpuAverageLoad } from '../interfaces/raspberry-pi';
import { Connection } from '../interfaces/connection';
import { DEFAULT_PAGE_SIZE } from '../constants/consts';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class CpuAverageLoadService {

  protected cpuAverageLoads$: BehaviorSubject<Connection<ICpuAverageLoad>> = new BehaviorSubject({
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
  searchQueryResult: Observable<Connection<ICpuAverageLoad>>;
  
  constructor(private apollo: Apollo,
    private errorHandlingService: ErrorHandlingService) {
  }

  getFirstCpuAverageLoads(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<ICpuAverageLoad>> {
    const variables = {
      firstAverageLoads: pageSize,
      afterAverageLoads: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ cpuAverageLoads: Connection<ICpuAverageLoad> }>({
        query: gql`
          query CpuAverageLoads($firstAverageLoads: Int, $afterAverageLoads: String) {
            raspberryPi {
              cpu {
                averageLoads(first: $firstAverageLoads, after: $afterAverageLoads) {
                  items {
                    lastMinute
                    last5Minutes
                    last15Minutes
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
            const connection = result.data.raspberryPi.cpu.averageLoads as Connection<ICpuAverageLoad>;
            this.beforeCursor = connection.pageInfo.startCursor;
            this.afterCursor = connection.pageInfo.endCursor;
            return connection;
          }),
          tap(c => this.cpuAverageLoads$.next(c)),
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
          afterAverageLoads: this.afterCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.cpu.averageLoads.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              cpu: {
                averageLoads: {
                  items: unionBy(prev.raspberryPi.cpu.averageLoads.items, fetchMoreResult.raspberryPi.cpu.averageLoads.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.cpu.averageLoads.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.cpu.averageLoads.totalCount,
                  __typename: 'CpuAverageLoadConnection'
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

  getLastCpuAverageLoads(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<ICpuAverageLoad>> {
    const variables = {
      lastAverageLoads: pageSize,
      beforeAverageLoads: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ cpuAverageLoads: Connection<ICpuAverageLoad> }>({
        query: gql`
          query CpuAverageLoads($lastAverageLoads: Int, $beforeAverageLoads: String) {
            raspberryPi {
              cpu {
                averageLoads(last: $lastAverageLoads, before: $beforeAverageLoads) {
                  items {
                    lastMinute
                    last5Minutes
                    last15Minutes
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
            const connection = result.data.raspberryPi.cpu.averageLoads as Connection<ICpuAverageLoad>;
            this.beforeCursor = connection.pageInfo.startCursor;
            this.afterCursor = connection.pageInfo.endCursor;
            return connection;
          }),
          tap(c => this.cpuAverageLoads$.next(c)),
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
          beforeAverageLoads: this.beforeCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.cpu.averageLoads.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              cpu: {
                averageLoads: {
                  items: unionBy(prev.raspberryPi.cpu.averageLoads.items, fetchMoreResult.raspberryPi.cpu.averageLoads.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.cpu.averageLoads.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.cpu.averageLoads.totalCount,
                  __typename: 'CpuAverageLoadConnection'
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

  subscribeToNewCpuAverageLoads() {
    this.searchQuery.subscribeToMore({
      document: gql`
      subscription CpuAverageLoad {
        cpuAverageLoad {
          lastMinute
          last5Minutes
          last15Minutes
          dateTime
        }
      }`,
      updateQuery: (prev, { subscriptionData }) => {
        if (!subscriptionData.data) {
          return prev;
        }
        const newCpuAverageLoad = subscriptionData.data.cpuAverageLoad;
        return Object.assign({}, prev, {
          raspberryPi: {
            cpu: {
              averageLoads: {
                items: [newCpuAverageLoad, ...prev.raspberryPi.cpu.averageLoads.items],
                pageInfo: prev.raspberryPi.cpu.averageLoads.pageInfo,
                totalCount: prev.raspberryPi.cpu.averageLoads.totalCount + 1,
                __typename: 'CpuAverageLoadConnection'
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
