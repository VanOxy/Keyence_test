import { useEffect, useState } from 'react';
import { Alert, Table, Typography } from 'antd';
import { useSearchParams } from 'react-router-dom';
import { salesRecordsApi } from '../api/salesRecordsApi';
import { getErrorMessage } from '../api/errors';
import type { SalesRecordListItem } from '../api/types';

export function SalesRecordsListPage() {
  const [searchParams] = useSearchParams();
  const salesReportId = searchParams.get('salesReportId') ?? undefined;

  const [loading, setLoading] = useState(true);
  const [records, setRecords] = useState<SalesRecordListItem[]>([]);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);
    salesRecordsApi
      .list(salesReportId)
      .then(setRecords)
      .catch((error) => setErrorMessage(getErrorMessage(error)))
      .finally(() => setLoading(false));
  }, [salesReportId]);

  return (
    <div style={{ maxWidth: 1200, margin: '40px auto', padding: '0 16px' }}>
      <Typography.Title level={3}>{salesReportId ? 'Report records' : 'All my records'}</Typography.Title>

      {errorMessage && <Alert style={{ marginBottom: 24 }} type="error" title={errorMessage} showIcon />}

      <Table
        rowKey="id"
        loading={loading}
        dataSource={records}
        pagination={{ pageSize: 20 }}
        scroll={{ x: true }}
        columns={[
          { title: 'Date', dataIndex: 'date', width: 110 },
          { title: 'Company', dataIndex: 'companyName' },
          { title: 'Contact', dataIndex: 'contactPerson' },
          { title: 'Phone', dataIndex: 'phone' },
          { title: 'Email', dataIndex: 'email' },
          { title: 'Region', dataIndex: 'region', width: 90 },
          { title: 'Product', dataIndex: 'product', width: 100 },
          { title: 'Units', dataIndex: 'unitsSold', width: 80, align: 'right' },
          { title: 'Unit price', dataIndex: 'unitPrice', width: 100, align: 'right' },
          { title: 'Revenue', dataIndex: 'revenue', width: 110, align: 'right' },
        ]}
      />
    </div>
  );
}
