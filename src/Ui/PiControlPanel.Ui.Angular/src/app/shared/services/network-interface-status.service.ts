import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { unionBy, forEach } from 'lodash';
import { Apollo, QueryRef } from 'apollo-angular';
import gql from 'graphql-tag';
import { INetworkInterfaceStatus } from '../interfaces/raspberry-pi';
import { Connection } from '../interfaces/connection';
import { DEFAULT_PAGE_SIZE } from '../constants/consts';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class NetworkInterfaceStatusService {
  searchQueries: { [interfaceName: string]: QueryRef<any> } = {};
  afterCursors: { [interfaceName: string]: string } = {};
  beforeCursors: { [interfaceName: string]: string } = {};
  searchQueryResults: { [interfaceName: string]: Observable<Connection<INetworkInterfaceStatus>> } = {};
  
  constructor(private apollo: Apollo,
    private errorHandlingService: ErrorHandlingService) {
  }

  getFirstNetworkInterfaceStatuses(interfaceName: string, pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<INetworkInterfaceStatus>> {
    const variables = {
      name: interfaceName,
      firstNetworkInterfaceStatuses: pageSize,
      afterNetworkInterfaceStatuses: null
    };
    if (!this.searchQueries[interfaceName]) {
      this.searchQueries[interfaceName] = this.apollo.watchQuery<{ networkInterfaceStatuses: Connection<INetworkInterfaceStatus> }>({
        query: gql`
          query NetworkInterfaceStatuses($name: String!, $firstNetworkInterfaceStatuses: Int, $afterNetworkInterfaceStatuses: String) {
            raspberryPi {
              network {
                networkInterface(name: $name) {
                  statuses(first: $firstNetworkInterfaceStatuses, after: $afterNetworkInterfaceStatuses) {
                    items {
                      networkInterfaceName
                      dateTime
                      receiveSpeed
                      sendSpeed
                      totalReceived
                      totalSent
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
      this.searchQueryResults[interfaceName] = this.searchQueries[interfaceName].valueChanges
        .pipe(
          map(result => {
            const connection = result.data.raspberryPi.network.networkInterface.statuses as Connection<INetworkInterfaceStatus>;
            this.beforeCursors[interfaceName] = connection.pageInfo.startCursor;
            this.afterCursors[interfaceName] = connection.pageInfo.endCursor;
            return connection;
          }),
          catchError((err) => this.errorHandlingService.handleError(err))
        );
    } else {
      this.searchQueries[interfaceName].setVariables(variables, true, true);
    }
    return this.searchQueryResults[interfaceName];
  }

  getNextPage() {
    forEach(this.searchQueries, (searchQuery, interfaceName) => {
      if (searchQuery) {
        searchQuery.fetchMore({
          variables: {
            name: interfaceName,
            afterNetworkInterfaceStatuses: this.afterCursors[interfaceName]
          },
          updateQuery: ((prev, { fetchMoreResult }) => {
            if (!fetchMoreResult.raspberryPi.network.networkInterface.statuses.items) {
              return prev;
            }
            return Object.assign({}, prev, {
              raspberryPi: {
                network: {
                  networkInterface: {
                    statuses: {
                      items: unionBy(prev.raspberryPi.network.networkInterface.statuses.items, fetchMoreResult.raspberryPi.network.networkInterface.statuses.items, 'dateTime'),
                      pageInfo: fetchMoreResult.raspberryPi.network.networkInterface.statuses.pageInfo,
                      totalCount: fetchMoreResult.raspberryPi.network.networkInterface.statuses.totalCount,
                      __typename: 'NetworkInterfaceStatusConnection'
                    },
                    __typename: 'NetworkInterfaceType'
                  },
                  __typename: 'NetworkType'
                },
                __typename: 'RaspberryPiType'
              }
            });
          })
        });
      }
    });
  }

  getLastNetworkInterfaceStatuses(interfaceName: string, pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<INetworkInterfaceStatus>> {
    const variables = {
      name: interfaceName,
      lastNetworkInterfaceStatuses: pageSize,
      beforeNetworkInterfaceStatuses: null
    };
    if (!this.searchQueries[interfaceName]) {
      this.searchQueries[interfaceName] = this.apollo.watchQuery<{ networkInterfaceStatuses: Connection<INetworkInterfaceStatus> }>({
        query: gql`
          query NetworkInterfaceStatuses($name: String!, $lastNetworkInterfaceStatuses: Int, $beforeNetworkInterfaceStatuses: String) {
            raspberryPi {
              network {
                networkInterface(name: $name) {
                  statuses(last: $lastNetworkInterfaceStatuses, before: $beforeNetworkInterfaceStatuses) {
                    items {
                      networkInterfaceName
                      dateTime
                      receiveSpeed
                      sendSpeed
                      totalReceived
                      totalSent
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
      this.searchQueryResults[interfaceName] = this.searchQueries[interfaceName].valueChanges
        .pipe(
          map(result => {
            const connection = result.data.raspberryPi.network.networkInterface.statuses as Connection<INetworkInterfaceStatus>;
            this.beforeCursors[interfaceName] = connection.pageInfo.startCursor;
            this.afterCursors[interfaceName] = connection.pageInfo.endCursor;
            return connection;
          }),
          catchError((err) => this.errorHandlingService.handleError(err))
        );
    } else {
      this.searchQueries[interfaceName].setVariables(variables, true, true);
    }
    return this.searchQueryResults[interfaceName];
  }

  getPreviousPage() {
    forEach(this.searchQueries, (searchQuery, interfaceName) => {
      if (searchQuery) {
        searchQuery.fetchMore({
          variables: {
            name: interfaceName,
            beforeNetworkInterfaceStatuses: this.beforeCursors[interfaceName]
          },
          updateQuery: ((prev, { fetchMoreResult }) => {
            if (!fetchMoreResult.raspberryPi.network.networkInterface.statuses.items) {
              return prev;
            }
            return Object.assign({}, prev, {
              raspberryPi: {
                network: {
                  networkInterface: {
                    statuses: {
                      items: unionBy(prev.raspberryPi.network.networkInterface.statuses.items, fetchMoreResult.raspberryPi.network.networkInterface.statuses.items, 'dateTime'),
                      pageInfo: fetchMoreResult.raspberryPi.network.networkInterface.statuses.pageInfo,
                      totalCount: fetchMoreResult.raspberryPi.network.networkInterface.statuses.totalCount,
                      __typename: 'NetworkInterfaceStatusConnection'
                    },
                    __typename: 'NetworkInterfaceType'
                  },
                  __typename: 'NetworkType'
                },
                __typename: 'RaspberryPiType'
              }
            });
          })
        });
      }
    });
  }

  subscribeToNewNetworkInterfaceStatuses(interfaceName: string) {
    if (this.searchQueries[interfaceName]) {
      this.searchQueries[interfaceName].subscribeToMore({
        variables: {
          networkInterfaceName: interfaceName
        },
        document: gql`
          subscription NetworkInterfaceStatus($networkInterfaceName: String!) {
            networkInterfaceStatus(networkInterfaceName: $networkInterfaceName) {
              networkInterfaceName
              receiveSpeed
              sendSpeed
              totalReceived
              totalSent
              dateTime
            }
          }`,
        updateQuery: (prev, { subscriptionData }) => {
          if (!subscriptionData.data) {
            return prev;
          }
          const newNetworkInterfaceStatus = subscriptionData.data.networkInterfaceStatus;
          return Object.assign({}, prev, {
            raspberryPi: {
              network: {
                networkInterface: {
                  statuses: {
                    items: [newNetworkInterfaceStatus, ...prev.raspberryPi.network.networkInterface.statuses.items],
                    pageInfo: prev.raspberryPi.network.networkInterface.statuses.pageInfo,
                    totalCount: prev.raspberryPi.network.networkInterface.statuses.totalCount + 1,
                    __typename: 'NetworkInterfaceStatusConnection'
                  },
                  __typename: 'NetworkInterfaceType'
                },
                __typename: 'NetworkType'
              },
              __typename: 'RaspberryPiType'
            }
          });
        }
      });
    }
  }

  refetch() {
    if (this.searchQueries) {
      forEach(this.searchQueries, (searchQuery, fileSystemName) => {
        searchQuery.refetch();
      });
    }
  }

}
