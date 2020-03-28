import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { unionBy } from 'lodash';
import { Apollo, QueryRef } from 'apollo-angular';
import gql from 'graphql-tag';
import { IRandomAccessMemoryStatus } from '../interfaces/raspberry-pi';
import { Connection } from '../interfaces/connection';
import { DEFAULT_PAGE_SIZE } from '../constants/consts';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class RamStatusService {

  protected memoryStatuses$: BehaviorSubject<Connection<IRandomAccessMemoryStatus>> = new BehaviorSubject({
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
  searchQueryResult: Observable<Connection<IRandomAccessMemoryStatus>>;
  
  constructor(private apollo: Apollo,
    private errorHandlingService: ErrorHandlingService) {
  }

  getFirstMemoryStatuses(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<IRandomAccessMemoryStatus>> {
    const variables = {
      firstMemoryStatuses: pageSize,
      afterMemoryStatuses: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ memoryStatuses: Connection<IRandomAccessMemoryStatus> }>({
        query: gql`
          query RamStatuses($firstMemoryStatuses: Int, $afterMemoryStatuses: String) {
            raspberryPi {
              ram {
                statuses(first: $firstMemoryStatuses, after: $afterMemoryStatuses) {
                  items {
                    used
                    free
                    diskCache
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
            const connection = result.data.raspberryPi.ram.statuses as Connection<IRandomAccessMemoryStatus>;
            this.beforeCursor = connection.pageInfo.startCursor;
            this.afterCursor = connection.pageInfo.endCursor;
            return connection;
          }),
          tap(c => this.memoryStatuses$.next(c)),
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
          afterMemoryStatuses: this.afterCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.ram.statuses.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              ram: {
                statuses: {
                  items: unionBy(prev.raspberryPi.ram.statuses.items, fetchMoreResult.raspberryPi.ram.statuses.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.ram.statuses.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.ram.statuses.totalCount,
                  __typename: 'MemoryStatusConnection'
                },
                __typename: 'MemoryType'
              },
              __typename: 'RaspberryPiType'
            }
          });
        })
      });
    }
  }

  getLastMemoryStatuses(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<IRandomAccessMemoryStatus>> {
    const variables = {
      lastMemoryStatuses: pageSize,
      beforeMemoryStatuses: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ memoryStatuses: Connection<IRandomAccessMemoryStatus> }>({
        query: gql`
          query RamStatuses($lastMemoryStatuses: Int, $beforeMemoryStatuses: String) {
            raspberryPi {
              ram {
                statuses(last: $lastMemoryStatuses, before: $beforeMemoryStatuses) {
                  items {
                    used
                    free
                    diskCache
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
            const connection = result.data.raspberryPi.ram.statuses as Connection<IRandomAccessMemoryStatus>;
            this.beforeCursor = connection.pageInfo.startCursor;
            this.afterCursor = connection.pageInfo.endCursor;
            return connection;
          }),
          tap(c => this.memoryStatuses$.next(c)),
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
          beforeMemoryStatuses: this.beforeCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.ram.statuses.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              ram: {
                statuses: {
                  items: unionBy(prev.raspberryPi.ram.statuses.items, fetchMoreResult.raspberryPi.ram.statuses.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.ram.statuses.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.ram.statuses.totalCount,
                  __typename: 'MemoryStatusConnection'
                },
                __typename: 'MemoryType'
              },
              __typename: 'RaspberryPiType'
            }
          });
        })
      });
    }
  }

  subscribeToNewMemoryStatuses() {
    this.searchQuery.subscribeToMore({
      document: gql`
      subscription RamStatus {
        ramStatus {
          used
          free
          diskCache
          dateTime
        }
      }`,
      updateQuery: (prev, { subscriptionData }) => {
        if (!subscriptionData.data) {
          return prev;
        }
        const newMemoryStatus = subscriptionData.data.ramStatus;
        return Object.assign({}, prev, {
          raspberryPi: {
            ram: {
              statuses: {
                items: [newMemoryStatus, ...prev.raspberryPi.ram.statuses.items],
                pageInfo: prev.raspberryPi.ram.statuses.pageInfo,
                totalCount: prev.raspberryPi.ram.statuses.totalCount + 1,
                __typename: 'MemoryStatusConnection'
              },
              __typename: 'MemoryType'
            },
            __typename: 'RaspberryPiType'
          }
        });
      }
    });
  }

}