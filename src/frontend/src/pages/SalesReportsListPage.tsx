import { useEffect, useState } from 'react';
import { Alert, Button, message, Space, Table, Typography } from 'antd';
import { DeleteOutlined, EyeOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { salesReportsApi } from '../api/salesReportsApi';
import { getErrorMessage } from '../api/errors';
import type { SalesReportListItem } from '../api/types';

export function SalesReportsListPage() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [reports, setReports] = useState<SalesReportListItem[]>([]);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    salesReportsApi
      .list()
      .then(setReports)
      .catch((error) => setErrorMessage(getErrorMessage(error)))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div style={{ maxWidth: 880, margin: '40px auto', padding: '0 16px' }}>
      <Typography.Title level={3}>Reports</Typography.Title>

      {errorMessage && <Alert style={{ marginBottom: 24 }} type="error" title={errorMessage} showIcon />}

      <Table
        rowKey="id"
        loading={loading}
        dataSource={reports}
        pagination={{ pageSize: 20 }}
        columns={[
          { title: 'File name', dataIndex: 'originalFileName' },
          {
            title: 'Uploaded',
            dataIndex: 'createdAtUtc',
            width: 220,
            render: (value: string) => new Date(value).toLocaleString(),
          },
          {
            title: 'Action',
            key: 'action',
            width: 100,
            render: (_, report: SalesReportListItem) => (
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
        ]}
      />
    </div>
  );
}
