import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { unionBy } from 'lodash';
import { Apollo, QueryRef } from 'apollo-angular';
import gql from 'graphql-tag';
import { IOsStatus } from '../interfaces/raspberry-pi';
import { Connection } from '../interfaces/connection';
import { DEFAULT_PAGE_SIZE } from '../constants/consts';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class OsStatusService {
  searchQuery: QueryRef<any>;
  afterCursor: string = null;
  beforeCursor: string = null;
  searchQueryResult: Observable<Connection<IOsStatus>>;
  
  constructor(private apollo: Apollo,
    private errorHandlingService: ErrorHandlingService) {
  }

  getFirstOsStatuses(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<IOsStatus>> {
    const variables = {
      firstOsStatuses: pageSize,
      afterOsStatuses: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ osStatuses: Connection<IOsStatus> }>({
        query: gql`
          query OsStatuses($firstOsStatuses: Int, $afterOsStatuses: String) {
            raspberryPi {
              os {
                statuses(first: $firstOsStatuses, after: $afterOsStatuses) {
                  items {
                    uptime
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
            const connection = result.data.raspberryPi.os.statuses as Connection<IOsStatus>;
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
          afterOsStatuses: this.afterCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.os.statuses.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              os: {
                statuses: {
                  items: unionBy(prev.raspberryPi.os.statuses.items, fetchMoreResult.raspberryPi.os.statuses.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.os.statuses.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.os.statuses.totalCount,
                  __typename: 'OsStatusConnection'
                },
                __typename: 'OsType'
              },
              __typename: 'RaspberryPiType'
            }
          });
        })
      });
    }
  }

  getLastOsStatuses(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<IOsStatus>> {
    const variables = {
      lastOsStatuses: pageSize,
      beforeOsStatuses: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ osStatuses: Connection<IOsStatus> }>({
        query: gql`
          query OsStatuses($lastOsStatuses: Int, $beforeOsStatuses: String) {
            raspberryPi {
              os {
                statuses(last: $lastOsStatuses, before: $beforeOsStatuses) {
                  items {
                    uptime
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
            const connection = result.data.raspberryPi.os.statuses as Connection<IOsStatus>;
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
          beforeOsStatuses: this.beforeCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.os.statuses.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              os: {
                statuses: {
                  items: unionBy(prev.raspberryPi.os.statuses.items, fetchMoreResult.raspberryPi.os.statuses.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.os.statuses.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.os.statuses.totalCount,
                  __typename: 'OsStatusConnection'
                },
                __typename: 'OsType'
              },
              __typename: 'RaspberryPiType'
            }
          });
        })
      });
    }
  }

  subscribeToNewOsStatuses() {
    if (this.searchQuery) {
      this.searchQuery.subscribeToMore({
        document: gql`
        subscription OsStatus {
          osStatus {
            uptime
            dateTime
          }
        }`,
        updateQuery: (prev, { subscriptionData }) => {
          if (!subscriptionData.data) {
            return prev;
          }
          const newOsStatus = subscriptionData.data.osStatus;
          return Object.assign({}, prev, {
            raspberryPi: {
              os: {
                statuses: {
                  items: [newOsStatus, ...prev.raspberryPi.os.statuses.items],
                  pageInfo: prev.raspberryPi.os.statuses.pageInfo,
                  totalCount: prev.raspberryPi.os.statuses.totalCount + 1,
                  __typename: 'OsStatusConnection'
                },
                __typename: 'OsType'
              },
              __typename: 'RaspberryPiType'
            }
          });
        }
      });
    }
  }

  refetch() {
    this.searchQuery.refetch()
  }

}
