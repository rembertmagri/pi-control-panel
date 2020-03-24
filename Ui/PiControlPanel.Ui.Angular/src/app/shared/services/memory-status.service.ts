import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { unionBy } from 'lodash';
import { Apollo, QueryRef } from 'apollo-angular';
import gql from 'graphql-tag';
import { IMemoryStatus } from '../interfaces/raspberry-pi';
import { Connection } from '../interfaces/connection';
import { DEFAULT_PAGE_SIZE } from '../constants/consts';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class MemoryStatusService {

  protected memoryStatuses$: BehaviorSubject<Connection<IMemoryStatus>> = new BehaviorSubject({
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
  searchQueryResult: Observable<Connection<IMemoryStatus>>;
  
  constructor(private apollo: Apollo,
    private errorHandlingService: ErrorHandlingService) {
  }

  getFirstMemoryStatuses(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<IMemoryStatus>> {
    const variables = {
      firstMemoryStatuses: pageSize,
      afterMemoryStatuses: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ memoryStatuses: Connection<IMemoryStatus> }>({
        query: gql`
          query MemoryStatuses($firstMemoryStatuses: Int, $afterMemoryStatuses: String) {
            raspberryPi {
              memory {
                statuses(first: $firstMemoryStatuses, after: $afterMemoryStatuses) {
                  items {
                    used
                    available
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
            const connection = result.data.raspberryPi.memory.statuses as Connection<IMemoryStatus>;
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
          if (!fetchMoreResult.raspberryPi.memory.statuses.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              memory: {
                statuses: {
                  items: unionBy(prev.raspberryPi.memory.statuses.items, fetchMoreResult.raspberryPi.memory.statuses.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.memory.statuses.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.memory.statuses.totalCount,
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

  getLastMemoryStatuses(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<IMemoryStatus>> {
    const variables = {
      lastMemoryStatuses: pageSize,
      beforeMemoryStatuses: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ memoryStatuses: Connection<IMemoryStatus> }>({
        query: gql`
          query MemoryStatuses($lastMemoryStatuses: Int, $beforeMemoryStatuses: String) {
            raspberryPi {
              memory {
                statuses(last: $lastMemoryStatuses, before: $beforeMemoryStatuses) {
                  items {
                    used
                    available
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
            const connection = result.data.raspberryPi.memory.statuses as Connection<IMemoryStatus>;
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
          if (!fetchMoreResult.raspberryPi.memory.statuses.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              memory: {
                statuses: {
                  items: unionBy(prev.raspberryPi.memory.statuses.items, fetchMoreResult.raspberryPi.memory.statuses.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.memory.statuses.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.memory.statuses.totalCount,
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
      subscription MemoryStatus {
        memoryStatus {
          used
          available
          dateTime
        }
      }`,
      updateQuery: (prev, { subscriptionData }) => {
        if (!subscriptionData.data) {
          return prev;
        }
        const newMemoryStatus = subscriptionData.data.memoryStatus;
        return Object.assign({}, prev, {
          raspberryPi: {
            memory: {
              statuses: {
                items: [newMemoryStatus, ...prev.raspberryPi.memory.statuses.items],
                pageInfo: prev.raspberryPi.memory.statuses.pageInfo,
                totalCount: prev.raspberryPi.memory.statuses.totalCount + 1,
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
