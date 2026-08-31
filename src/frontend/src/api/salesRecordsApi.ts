import { httpClient } from './httpClient';
import type { SalesRecordListItem } from './types';

export const salesRecordsApi = {
  list: (salesReportId?: string) =>
    httpClient
      .get<SalesRecordListItem[]>('/api/sales-records', { params: { salesReportId } })
      .then((res) => res.data),
};
