import { Injectable } from '@angular/core';
import { Observable, timer } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { unionBy, forEach, isNil } from 'lodash';
import { Apollo, QueryRef } from 'apollo-angular';
import gql from 'graphql-tag';
import { IFileSystemStatus } from '../interfaces/raspberry-pi';
import { Connection } from '../interfaces/connection';
import { DEFAULT_PAGE_SIZE, QUERY_REFETCH_DUE_TIME, QUERY_REFETCH_PERIOD } from '../constants/consts';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class FileSystemStatusService {
  searchQueries: { [interfaceName: string]: QueryRef<any> } = {};
  afterCursors: { [interfaceName: string]: string } = {};
  beforeCursors: { [interfaceName: string]: string } = {};
  searchQueryResults: { [interfaceName: string]: Observable<Connection<IFileSystemStatus>> } = {};
  
  constructor(private apollo: Apollo,
    private errorHandlingService: ErrorHandlingService) {
  }

  getFirstFileSystemStatuses(fileSystemName: string, pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<IFileSystemStatus>> {
    const variables = {
      name: fileSystemName,
      firstFileSystemStatuses: pageSize,
      afterFileSystemStatuses: null
    };
    if (!this.searchQueries[fileSystemName]) {
      this.searchQueries[fileSystemName] = this.apollo.watchQuery<{ fileSystemStatuses: Connection<IFileSystemStatus> }>({
        query: gql`
          query FileSystemStatuses($name: String!, $firstFileSystemStatuses: Int, $afterFileSystemStatuses: String) {
            raspberryPi {
              disk {
                fileSystem(name: $name) {
                  statuses(first: $firstFileSystemStatuses, after: $afterFileSystemStatuses) {
                    items {
                      fileSystemName
                      available
                      used
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
            }
          }`,
        variables,
        fetchPolicy: 'network-only'
      });
      this.searchQueryResults[fileSystemName] = this.searchQueries[fileSystemName].valueChanges
        .pipe(
          map(result => {
            const connection = result.data.raspberryPi.disk.fileSystem.statuses as Connection<IFileSystemStatus>;
            this.beforeCursors[fileSystemName] = connection.pageInfo.startCursor;
            this.afterCursors[fileSystemName] = connection.pageInfo.endCursor;
            return connection;
          }),
          catchError((err) => this.errorHandlingService.handleError(err))
        );
    } else {
      this.searchQueries[fileSystemName].setVariables(variables, true, true);
    }
    return this.searchQueryResults[fileSystemName];
  }

  getNextPage() {
    forEach(this.searchQueries, (searchQuery, fileSystemName) => {
      if (searchQuery) {
        searchQuery.fetchMore({
          variables: {
            afterFileSystemStatuses: this.afterCursors[fileSystemName]
          },
          updateQuery: ((prev, { fetchMoreResult }) => {
            if (!fetchMoreResult.raspberryPi.disk.fileSystem.statuses.items) {
              return prev;
            }
            return Object.assign({}, prev, {
              raspberryPi: {
                disk: {
                  fileSystem: {
                    statuses: {
                      items: unionBy(prev.raspberryPi.disk.statuses.items, fetchMoreResult.raspberryPi.disk.statuses.items, 'dateTime'),
                      pageInfo: fetchMoreResult.raspberryPi.disk.statuses.pageInfo,
                      totalCount: fetchMoreResult.raspberryPi.disk.statuses.totalCount,
                      __typename: 'FileSystemStatusConnection'
                    },
                    __typename: 'FileSystemType'
                  },
                  __typename: 'DiskType'
                },
                __typename: 'RaspberryPiType'
              }
            });
          })
        });
      }
    });
  }

  getLastFileSystemStatuses(fileSystemName: string, pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<IFileSystemStatus>> {
    const variables = {
      name: fileSystemName,
      lastFileSystemStatuses: pageSize,
      beforeFileSystemStatuses: null
    };
    if (!this.searchQueries[fileSystemName]) {
      this.searchQueries[fileSystemName] = this.apollo.watchQuery<{ fileSystemStatuses: Connection<IFileSystemStatus> }>({
        query: gql`
          query FileSystemStatuses($name: String!, $lastFileSystemStatuses: Int, $beforeFileSystemStatuses: String) {
            raspberryPi {
              disk {
                fileSystem(name: $name) {
                  statuses(last: $lastFileSystemStatuses, before: $beforeFileSystemStatuses) {
                    items {
                      fileSystemName
                      available
                      used
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
            }
          }`,
        variables,
        fetchPolicy: 'network-only'
      });
      this.searchQueryResults[fileSystemName] = this.searchQueries[fileSystemName].valueChanges
        .pipe(
          map(result => {
            const connection = result.data.raspberryPi.disk.fileSystem.statuses as Connection<IFileSystemStatus>;
            this.beforeCursors[fileSystemName] = connection.pageInfo.startCursor;
            this.afterCursors[fileSystemName] = connection.pageInfo.endCursor;
            return connection;
          }),
          catchError((err) => this.errorHandlingService.handleError(err))
        );
    } else {
      this.searchQueries[fileSystemName].setVariables(variables, true, true);
    }
    return this.searchQueryResults[fileSystemName];
  }

  getPreviousPage() {
    forEach(this.searchQueries, (searchQuery, fileSystemName) => {
      if (searchQuery) {
        searchQuery.fetchMore({
          variables: {
            beforeFileSystemStatuses: this.beforeCursors[fileSystemName]
          },
          updateQuery: ((prev, { fetchMoreResult }) => {
            if (!fetchMoreResult.raspberryPi.disk.fileSystem.statuses.items) {
              return prev;
            }
            return Object.assign({}, prev, {
              raspberryPi: {
                disk: {
                  fileSystem: {
                    statuses: {
                      items: unionBy(prev.raspberryPi.disk.statuses.items, fetchMoreResult.raspberryPi.disk.statuses.items, 'dateTime'),
                      pageInfo: fetchMoreResult.raspberryPi.disk.statuses.pageInfo,
                      totalCount: fetchMoreResult.raspberryPi.disk.statuses.totalCount,
                      __typename: 'FileSystemStatusConnection'
                    },
                    __typename: 'FileSystemType'
                  },
                  __typename: 'DiskType'
                },
                __typename: 'RaspberryPiType'
              }
            });
          })
        });
      }
    });
  }

  subscribeToNewFileSystemStatuses(fileSystemName: string) {
    if (this.searchQueries[fileSystemName]) {
      this.searchQueries[fileSystemName].subscribeToMore({
        variables: {
          fileSystemName: fileSystemName
        },
        document: gql`
          subscription FileSystemStatus($fileSystemName: String!) {
            fileSystemStatus(fileSystemName: $fileSystemName) {
              fileSystemName
              available
              used
              dateTime
            }
          }`,
        updateQuery: (prev, { subscriptionData }) => {
          if (!subscriptionData.data) {
            return prev;
          }
          const newFileSystemStatus = subscriptionData.data.fileSystemStatus;
          return Object.assign({}, prev, {
            raspberryPi: {
              disk: {
                fileSystem: {
                  statuses: {
                    items: [newFileSystemStatus, ...prev.raspberryPi.disk.fileSystem.statuses.items],
                    pageInfo: prev.raspberryPi.disk.fileSystem.statuses.pageInfo,
                    totalCount: prev.raspberryPi.disk.fileSystem.statuses.totalCount + 1,
                    __typename: 'FileSystemStatusConnection'
                  },
                  __typename: 'FileSystemType'
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

  refetchPeriodically(): Observable<boolean> {
    return timer(QUERY_REFETCH_DUE_TIME, QUERY_REFETCH_PERIOD)
      .pipe(
        tap(() => {
          if (this.searchQueries) {
            forEach(this.searchQueries, (searchQuery, fileSystemName) => {
              searchQuery.resetLastResults();
              searchQuery.refetch();
            });
          }
        }),
        map(result => !isNil(result))
      );
  }

  refetch() {
    if (this.searchQueries) {
      forEach(this.searchQueries, (searchQuery, fileSystemName) => {
        searchQuery.refetch();
      });
    }
  }

}
