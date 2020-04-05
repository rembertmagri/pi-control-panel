import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { unionBy } from 'lodash';
import { Apollo, QueryRef } from 'apollo-angular';
import gql from 'graphql-tag';
import { IDiskStatus } from '../interfaces/raspberry-pi';
import { Connection } from '../interfaces/connection';
import { DEFAULT_PAGE_SIZE } from '../constants/consts';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class DiskStatusService {

  protected diskStatuses$: BehaviorSubject<Connection<IDiskStatus>> = new BehaviorSubject({
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
  searchQueryResult: Observable<Connection<IDiskStatus>>;
  
  constructor(private apollo: Apollo,
    private errorHandlingService: ErrorHandlingService) {
  }

  getFirstDiskStatuses(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<IDiskStatus>> {
    const variables = {
      firstDiskStatuses: pageSize,
      afterDiskStatuses: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ diskStatuses: Connection<IDiskStatus> }>({
        query: gql`
          query DiskStatuses($firstDiskStatuses: Int, $afterDiskStatuses: String) {
            raspberryPi {
              disk {
                statuses(first: $firstDiskStatuses, after: $afterDiskStatuses) {
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
            const connection = result.data.raspberryPi.disk.statuses as Connection<IDiskStatus>;
            this.beforeCursor = connection.pageInfo.startCursor;
            this.afterCursor = connection.pageInfo.endCursor;
            return connection;
          }),
          tap(c => this.diskStatuses$.next(c)),
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
          afterDiskStatuses: this.afterCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.disk.statuses.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              disk: {
                statuses: {
                  items: unionBy(prev.raspberryPi.disk.statuses.items, fetchMoreResult.raspberryPi.disk.statuses.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.disk.statuses.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.disk.statuses.totalCount,
                  __typename: 'DiskStatusConnection'
                },
                __typename: 'DiskType'
              },
              __typename: 'RaspberryPiType'
            }
          });
        })
      });
    }
  }

  getLastDiskStatuses(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<IDiskStatus>> {
    const variables = {
      lastDiskStatuses: pageSize,
      beforeDiskStatuses: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ diskStatuses: Connection<IDiskStatus> }>({
        query: gql`
          query DiskStatuses($lastDiskStatuses: Int, $beforeDiskStatuses: String) {
            raspberryPi {
              disk {
                statuses(last: $lastDiskStatuses, before: $beforeDiskStatuses) {
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
            const connection = result.data.raspberryPi.disk.statuses as Connection<IDiskStatus>;
            this.beforeCursor = connection.pageInfo.startCursor;
            this.afterCursor = connection.pageInfo.endCursor;
            return connection;
          }),
          tap(c => this.diskStatuses$.next(c)),
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
          beforeDiskStatuses: this.beforeCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.disk.statuses.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              disk: {
                statuses: {
                  items: unionBy(prev.raspberryPi.disk.statuses.items, fetchMoreResult.raspberryPi.disk.statuses.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.disk.statuses.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.disk.statuses.totalCount,
                  __typename: 'DiskStatusConnection'
                },
                __typename: 'DiskType'
              },
              __typename: 'RaspberryPiType'
            }
          });
        })
      });
    }
  }

  subscribeToNewDiskStatuses() {
    this.searchQuery.subscribeToMore({
      document: gql`
      subscription DiskStatus {
        diskStatus {
          used
          available
          dateTime
        }
      }`,
      updateQuery: (prev, { subscriptionData }) => {
        if (!subscriptionData.data) {
          return prev;
        }
        const newDiskStatus = subscriptionData.data.diskStatus;
        return Object.assign({}, prev, {
          raspberryPi: {
            disk: {
              statuses: {
                items: [newDiskStatus, ...prev.raspberryPi.disk.statuses.items],
                pageInfo: prev.raspberryPi.disk.statuses.pageInfo,
                totalCount: prev.raspberryPi.disk.statuses.totalCount + 1,
                __typename: 'DiskStatusConnection'
              },
              __typename: 'DiskType'
            },
            __typename: 'RaspberryPiType'
          }
        });
      }
    });
  }

}
