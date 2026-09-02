import { useState } from 'react';
import { useQuery, keepPreviousData } from '@tanstack/react-query';
import { Alert, Button, Input, message, Space, Table, Typography } from 'antd';
import type { SorterResult } from 'antd/es/table/interface';
import { DeleteOutlined, EyeOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { salesReportsApi } from '../api/salesReportsApi';
import { getErrorMessage } from '../api/errors';
import { useAuth } from '../auth/AuthContext';
import type { SalesReportListItem } from '../api/types';

const PAGE_SIZE = 50;

export function SalesReportsListPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const isManagerOrAdmin = user?.role === 'Manager' || user?.role === 'Admin';

  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [sortBy, setSortBy] = useState<string>();
  const [descending, setDescending] = useState(true);

  const {
    data,
    isLoading,
    isPlaceholderData,
    error,
  } = useQuery({
    queryKey: ['salesReports', search, sortBy, descending, page],
    queryFn: () =>
      salesReportsApi.list({ search: search || undefined, sortBy, descending, skip: (page - 1) * PAGE_SIZE, take: PAGE_SIZE }),
    placeholderData: keepPreviousData, // keep showing the current page while the next one loads
  });

  const handleTableChange = (
    pagination: { current?: number },
    _filters: unknown,
    sorter: SorterResult<SalesReportListItem> | SorterResult<SalesReportListItem>[],
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
    ...(isManagerOrAdmin
      ? [{ title: 'Owner', dataIndex: 'ownerFullName', width: 180, sorter: true as const }]
      : []),
    { title: 'File name', dataIndex: 'originalFileName', sorter: true as const },
    {
      title: 'Uploaded',
      dataIndex: 'createdAtUtc',
      width: 220,
      sorter: true as const,
      defaultSortOrder: 'descend' as const,
      render: (value: string) => new Date(value).toLocaleString(),
    },
    {
      title: 'Action',
      key: 'action',
      width: 100,
      render: (_: unknown, report: SalesReportListItem) => (
        <Space>
          <Button
            type="text"
            icon={<EyeOutlined />}
            onClick={() => navigate(`/records?salesReportId=${report.id}`)}
          />
          <Button
            type="text"
            danger
            icon={<DeleteOutlined />}
            onClick={() => message.info(`Delete ${report.originalFileName} — not implemented yet`)}
          />
        </Space>
      ),
    },
  ];

  return (
    <div style={{ maxWidth: 1000, margin: '40px auto', padding: '0 16px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <Typography.Title level={3} style={{ margin: 0 }}>
          Reports
        </Typography.Title>

        <Input.Search
          placeholder={isManagerOrAdmin ? 'Search by file name or owner' : 'Search by file name'}
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
        columns={columns}
        onChange={handleTableChange}
      />
    </div>
  );
}