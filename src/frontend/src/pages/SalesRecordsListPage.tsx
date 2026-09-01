import { useState } from 'react';
import { useQuery, keepPreviousData } from '@tanstack/react-query';
import { Alert, Input, Table, Typography } from 'antd';
import type { SorterResult } from 'antd/es/table/interface';
import { useSearchParams } from 'react-router-dom';
import { salesRecordsApi } from '../api/salesRecordsApi';
import { getErrorMessage } from '../api/errors';
import type { SalesRecordListItem } from '../api/types';

const PAGE_SIZE = 50;

export function SalesRecordsListPage() {
  const [searchParams] = useSearchParams();
  const salesReportId = searchParams.get('salesReportId') ?? undefined;

  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  // Sorting/paging happen on the server — this only tracks which page/column/direction to ask for.
  const [sortBy, setSortBy] = useState<string>();
  const [descending, setDescending] = useState(true);

  const {
    data,
    isLoading,
    isPlaceholderData,
    error,
  } = useQuery({
    queryKey: ['salesRecords', salesReportId, search, sortBy, descending, page],
    queryFn: () =>
      salesRecordsApi.list({
        salesReportId,
        search: search || undefined,
        sortBy,
        descending,
        skip: (page - 1) * PAGE_SIZE,
        take: PAGE_SIZE,
      }),
    placeholderData: keepPreviousData, // keep showing the current page while the next one loads
  });

  const handleTableChange = (
    pagination: { current?: number },
    _filters: unknown,
    sorter: SorterResult<SalesRecordListItem> | SorterResult<SalesRecordListItem>[],
  ) => {
    setPage(pagination.current ?? 1);

    const { field, order } = Array.isArray(sorter) ? sorter[0] : sorter;
    if (typeof field === 'string' && order) {
      setSortBy(field.charAt(0).toUpperCase() + field.slice(1)); // dataIndex is camelCase, backend property is PascalCase
      setDescending(order === 'descend');
    }
  };

  const handleSearch = (value: string) => {
    setSearch(value);
    setPage(1); // a new search always starts back at page 1
  };

  const columns = [
    { title: 'Date', dataIndex: 'date', width: 110, sorter: true as const, defaultSortOrder: 'descend' as const },
    { title: 'Company', dataIndex: 'companyName', sorter: true as const },
    { title: 'Contact', dataIndex: 'contactPerson', sorter: true as const },
    { title: 'Phone', dataIndex: 'phone', sorter: true as const },
    { title: 'Email', dataIndex: 'email', sorter: true as const },
    { title: 'Region', dataIndex: 'region', width: 90, sorter: true as const },
    { title: 'Product', dataIndex: 'product', width: 100, sorter: true as const },
    { title: 'Units', dataIndex: 'unitsSold', width: 80, align: 'right' as const, sorter: true as const },
    { title: 'Unit price', dataIndex: 'unitPrice', width: 100, align: 'right' as const, sorter: true as const },
    { title: 'Revenue', dataIndex: 'revenue', width: 110, align: 'right' as const, sorter: true as const },
  ];

  return (
    <div style={{ maxWidth: 1200, margin: '40px auto', padding: '0 16px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <Typography.Title level={3} style={{ margin: 0 }}>
          {salesReportId ? 'Report records' : 'All my records'}
        </Typography.Title>

        <Input.Search
          placeholder="Search by company, contact or product"
          allowClear
          style={{ width: 280 }}
          onSearch={handleSearch}
        />
      </div>

      {error && <Alert style={{ marginBottom: 24 }} type="error" title={getErrorMessage(error)} showIcon />}

      <Table
        rowKey="id"
        size="small"
        loading={isLoading || isPlaceholderData}
        dataSource={data?.items ?? []}
        pagination={{ current: page, pageSize: PAGE_SIZE, total: data?.totalCount ?? 0 }}
        scroll={{ x: true }}
        columns={columns}
        onChange={handleTableChange}
      />
    </div>
  );
}
