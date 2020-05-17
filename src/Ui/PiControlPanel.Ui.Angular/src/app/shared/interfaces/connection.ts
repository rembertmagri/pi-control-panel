export interface Connection<T> {
  items: T[];
  totalCount: number;
  pageInfo: PageInfo;
}

export interface PageInfo {
  endCursor: string;
  hasNextPage: boolean;
  startCursor: string;
  hasPreviousPage: boolean;
}
