import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { unionBy } from 'lodash';
import { Apollo, QueryRef } from 'apollo-angular';
import gql from 'graphql-tag';
import { ICpuFrequency } from '../interfaces/raspberry-pi';
import { Connection } from '../interfaces/connection';
import { DEFAULT_PAGE_SIZE } from '../constants/consts';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class CpuFrequencyService {
  searchQuery: QueryRef<any>;
  afterCursor: string = null;
  beforeCursor: string = null;
  searchQueryResult: Observable<Connection<ICpuFrequency>>;
  
  constructor(private apollo: Apollo,
    private errorHandlingService: ErrorHandlingService) {
  }

  getFirstCpuFrequencies(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<ICpuFrequency>> {
    const variables = {
      firstFrequencies: pageSize,
      afterFrequencies: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ cpuFrequencies: Connection<ICpuFrequency> }>({
        query: gql`
          query CpuFrequencies($firstFrequencies: Int, $afterFrequencies: String) {
            raspberryPi {
              cpu {
                frequencies(first: $firstFrequencies, after: $afterFrequencies) {
                items {
                    value
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
            const connection = result.data.raspberryPi.cpu.frequencies as Connection<ICpuFrequency>;
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
          afterFrequencies: this.afterCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.cpu.frequencies.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              cpu: {
                frequencies: {
                  items: unionBy(prev.raspberryPi.cpu.frequencies.items, fetchMoreResult.raspberryPi.cpu.frequencies.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.cpu.frequencies.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.cpu.frequencies.totalCount,
                  __typename: 'CpuFrequencyConnection'
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

  getLastCpuFrequencies(pageSize: number = DEFAULT_PAGE_SIZE): Observable<Connection<ICpuFrequency>> {
    const variables = {
      lastFrequencies: pageSize,
      beforeFrequencies: null
    };
    if (!this.searchQuery) {
      this.searchQuery = this.apollo.watchQuery<{ cpuFrequencies: Connection<ICpuFrequency> }>({
        query: gql`
          query CpuFrequencies($lastFrequencies: Int, $beforeFrequencies: String) {
            raspberryPi {
              cpu {
                frequencies(last: $lastFrequencies, before: $beforeFrequencies) {
                items {
                    value
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
            const connection = result.data.raspberryPi.cpu.frequencies as Connection<ICpuFrequency>;
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
          beforeFrequencies: this.beforeCursor
        },
        updateQuery: ((prev, { fetchMoreResult }) => {
          if (!fetchMoreResult.raspberryPi.cpu.frequencies.items) {
            return prev;
          }
          return Object.assign({}, prev, {
            raspberryPi: {
              cpu: {
                frequencies: {
                  items: unionBy(prev.raspberryPi.cpu.frequencies.items, fetchMoreResult.raspberryPi.cpu.frequencies.items, 'dateTime'),
                  pageInfo: fetchMoreResult.raspberryPi.cpu.frequencies.pageInfo,
                  totalCount: fetchMoreResult.raspberryPi.cpu.frequencies.totalCount,
                  __typename: 'CpuFrequencyConnection'
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

  subscribeToNewCpuFrequencies() {
    if (this.searchQuery) {
      this.searchQuery.subscribeToMore({
        document: gql`
        subscription CpuFrequency {
          cpuFrequency {
            value
            dateTime
          }
        }`,
        updateQuery: (prev, { subscriptionData }) => {
          if (!subscriptionData.data) {
            return prev;
          }
          const newCpuFrequency = subscriptionData.data.cpuFrequency;
          return Object.assign({}, prev, {
            raspberryPi: {
              cpu: {
                frequencies: {
                  items: [newCpuFrequency, ...prev.raspberryPi.cpu.frequencies.items],
                  pageInfo: prev.raspberryPi.cpu.frequencies.pageInfo,
                  totalCount: prev.raspberryPi.cpu.frequencies.totalCount + 1,
                  __typename: 'CpuFrequencyConnection'
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

  refetch() {
    if (this.searchQuery) {
      this.searchQuery.refetch();
    }
  }

}
