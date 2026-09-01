import { gql } from 'graphql-request';
import { gqlClient } from './graphqlClient';
import type { SalesRecordsPage } from './types';

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
};
