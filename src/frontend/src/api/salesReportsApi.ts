import { gql } from 'graphql-request';
import { httpClient } from './httpClient';
import { gqlClient } from './graphqlClient';
import type { ImportResult, SalesReportsPage } from './types';

const SALES_REPORTS_QUERY = gql`
  query SalesReports($search: String, $sortBy: String, $descending: Boolean, $skip: Int, $take: Int) {
    salesReports(search: $search, sortBy: $sortBy, descending: $descending, skip: $skip, take: $take) {
      totalCount
      items {
        id
        originalFileName
        createdAtUtc
        ownerId
        ownerFullName
      }
    }
  }
`;

export interface SalesReportsListParams {
  search?: string;
  sortBy?: string;
  descending?: boolean;
  skip?: number;
  take?: number;
}

export const salesReportsApi = {
  // Import stays REST — it's a file upload (multipart), not a fit for GraphQL.
  import: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);

    return httpClient
      .post<ImportResult>('/api/sales-reports', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      })
      .then((res) => res.data);
  },

  list: (params: SalesReportsListParams) =>
    gqlClient
      .request<{ salesReports: SalesReportsPage }>(SALES_REPORTS_QUERY, params)
      .then((data) => data.salesReports),

  delete: (id: string) => httpClient.delete(`/api/sales-reports/${id}`),
};