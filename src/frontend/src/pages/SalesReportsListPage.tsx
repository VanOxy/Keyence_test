import { useState } from 'react';
import { useMutation, useQuery, useQueryClient, keepPreviousData } from '@tanstack/react-query';
import { Alert, Button, Input, message, Popconfirm, Space, Table, Typography } from 'antd';
import type { SorterResult } from 'antd/es/table/interface';
import { DeleteOutlined, EyeOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { salesReportsApi } from '../api/salesReportsApi';
import { getErrorMessage } from '../api/errors';
import { useAuth } from '../auth/AuthContext';
import type { SalesReportListItem } from '../api/types';

export function SalesReportsListPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { user } = useAuth();
  const isManagerOrAdmin = user?.role === 'Manager' || user?.role === 'Admin';

  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(50);
  const [sortBy, setSortBy] = useState<string>();
  const [descending, setDescending] = useState(true);

  const {
    data,
    isLoading,
    isPlaceholderData,
    error,
  } = useQuery({
    queryKey: ['salesReports', search, sortBy, descending, page, pageSize],
    queryFn: () =>
      salesReportsApi.list({ search: search || undefined, sortBy, descending, skip: (page - 1) * pageSize, take: pageSize }),
    placeholderData: keepPreviousData, // keep showing the current page while the next one loads
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => salesReportsApi.delete(id),
    onSuccess: () => {
      message.success('Report deleted.');
      queryClient.invalidateQueries({ queryKey: ['salesReports'] });
    },
    onError: (err) => message.error(getErrorMessage(err)),
  });

  const handleTableChange = (
    pagination: { current?: number; pageSize?: number },
    _filters: unknown,
    sorter: SorterResult<SalesReportListItem> | SorterResult<SalesReportListItem>[],
  ) => {
    if (pagination.pageSize && pagination.pageSize !== pageSize) {
      setPageSize(pagination.pageSize);
      setPage(1); // page-size change always starts back at page 1 — the old page number no longer means the same rows
    } else {
      setPage(pagination.current ?? 1);
    }

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
          <Popconfirm
            title={`Delete ${report.originalFileName}?`}
            description="This also deletes all of its records."
            onConfirm={() => deleteMutation.mutate(report.id)}
            okText="Delete"
            okButtonProps={{ danger: true }}
          >
            <Button type="text" danger icon={<DeleteOutlined />} loading={deleteMutation.isPending} />
          </Popconfirm>
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
        pagination={{
          current: page,
          pageSize,
          total: data?.totalCount ?? 0,
          showSizeChanger: true,
          pageSizeOptions: [20, 50, 100, 200],
        }}
        columns={columns}
        onChange={handleTableChange}
      />
    </div>
  );
}