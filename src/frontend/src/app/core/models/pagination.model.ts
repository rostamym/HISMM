export interface PaginationParams {
  page: number;
  pageSize: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export class PaginationHelper {
  static createParams(
    page: number = 1,
    pageSize: number = 10,
    sortBy?: string,
    sortDirection: 'asc' | 'desc' = 'asc'
  ): PaginationParams {
    return { page, pageSize, sortBy, sortDirection };
  }

  static getTotalPages(totalCount: number, pageSize: number): number {
    return Math.ceil(totalCount / pageSize);
  }
}
