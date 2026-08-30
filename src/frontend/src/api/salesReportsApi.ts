import { httpClient } from './httpClient';
import type { ImportResult, SalesReportListItem } from './types';

export const salesReportsApi = {
  import: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);

    return httpClient
      .post<ImportResult>('/api/sales-reports', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      })
      .then((res) => res.data);
  },

  list: () => httpClient.get<SalesReportListItem[]>('/api/sales-reports').then((res) => res.data),
};
