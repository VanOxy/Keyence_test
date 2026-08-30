import { useState } from 'react';
import { Alert, Button, Card, Col, Row, Statistic, Table, Typography, Upload } from 'antd';
import type { UploadProps } from 'antd';
import { salesReportsApi } from '../api/salesReportsApi';
import { getErrorMessage } from '../api/errors';
import type { ImportResult } from '../api/types';

export function UploadPage() {
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<ImportResult | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const handleUpload: UploadProps['beforeUpload'] = (file) => {
    setLoading(true);
    setResult(null);
    setErrorMessage(null);

    salesReportsApi
      .import(file)
      .then(setResult)
      .catch((error) => setErrorMessage(getErrorMessage(error)))
      .finally(() => setLoading(false));

    return false; // we handle the upload ourselves — don't let AntD auto-POST the file
  };

  return (
    <div style={{ maxWidth: 720, margin: '40px auto', padding: '0 16px' }}>
      <Typography.Title level={3}>Import sales report</Typography.Title>

      <Upload beforeUpload={handleUpload} showUploadList={false} accept=".xlsx" disabled={loading}>
        <Button loading={loading}>Select .xlsx file</Button>
      </Upload>

      {errorMessage && (
        <Alert style={{ marginTop: 24 }} type="error" message={errorMessage} showIcon />
      )}

      {result && (
        <Card style={{ marginTop: 24 }}>
          <Row gutter={16}>
            <Col span={8}>
              <Statistic title="Total" value={result.totalRows} />
            </Col>
            <Col span={8}>
              <Statistic title="Valid" value={result.validRows} valueStyle={{ color: '#3f8600' }} />
            </Col>
            <Col span={8}>
              <Statistic
                title="Errors"
                value={result.invalidRows}
                valueStyle={result.invalidRows > 0 ? { color: '#cf1322' } : undefined}
              />
            </Col>
          </Row>

          {result.success ? (
            <Alert
              style={{ marginTop: 16 }}
              type="success"
              showIcon
              message="Import successful — all rows were saved."
            />
          ) : (
            <>
              <Alert
                style={{ marginTop: 16 }}
                type="error"
                showIcon
                message="Import failed — nothing was saved. Fix the rows below and re-upload."
              />
              <Table
                style={{ marginTop: 16 }}
                size="small"
                rowKey={(row) => `${row.rowNumber}-${row.column}`}
                dataSource={result.errors}
                pagination={{ pageSize: 10 }}
                columns={[
                  { title: 'Row', dataIndex: 'rowNumber', width: 80 },
                  { title: 'Column', dataIndex: 'column', width: 160 },
                  { title: 'Error', dataIndex: 'message' },
                ]}
              />
            </>
          )}
        </Card>
      )}
    </div>
  );
}
