import { gql } from 'graphql-request';
import { httpClient } from './httpClient';
import { gqlClient } from './graphqlClient';
import type { SalesRecordListItem, SalesRecordsPage, UpdateSalesRecordInput } from './types';

const SALES_RECORDS_QUERY = gql`
  query SalesRecords($search: String, $salesReportId: UUID, $sortBy: String, $descending: Boolean, $skip: Int, $take: Int) {
    salesRecords(
      search: $search
      salesReportId: $salesReportId
      sortBy: $sortBy
      descending: $descending
      skip: $skip
      take: $take
    ) {
      totalCount
      items {
        id
        date
        companyName
        contactPerson
        phone
        email
        region
        product
        unitsSold
        unitPrice
        revenue
      }
    }
  }
`;

export interface SalesRecordsListParams {
  search?: string;
  salesReportId?: string;
  sortBy?: string; // one of SalesRecordListItem's property names, e.g. "CompanyName"
  descending?: boolean;
  skip?: number;
  take?: number;
}

export const salesRecordsApi = {
  list: (params: SalesRecordsListParams) =>
    gqlClient
      .request<{ salesRecords: SalesRecordsPage }>(SALES_RECORDS_QUERY, params)
      .then((data) => data.salesRecords),

  // Get/update/delete stay REST — same controller, one record at a time, not a fit for GraphQL.
  getById: (id: number) => httpClient.get<SalesRecordListItem>(`/api/sales-records/${id}`).then((res) => res.data),

  update: (id: number, input: UpdateSalesRecordInput) =>
    httpClient.put<SalesRecordListItem>(`/api/sales-records/${id}`, input).then((res) => res.data),

  delete: (id: number) => httpClient.delete(`/api/sales-records/${id}`),
};
