import { useState } from 'react';
import { useMutation, useQuery } from '@tanstack/react-query';
import { Alert, Button, Col, DatePicker, Form, Input, InputNumber, Row, Spin, Typography } from 'antd';
import dayjs from 'dayjs';
import { useParams } from 'react-router-dom';
import { salesRecordsApi } from '../api/salesRecordsApi';
import { getErrorMessage } from '../api/errors';
import type { UpdateSalesRecordInput } from '../api/types';

// Opened via window.open() from the records table (a new tab) — it has no in-memory state from
// the page that opened it, so it independently loads the record by id and posts back on save.
export function SalesRecordEditPage() {
  const { id } = useParams<{ id: string }>();
  const recordId = Number(id);
  const [saved, setSaved] = useState(false);

  const { data: record, isLoading, error } = useQuery({
    queryKey: ['salesRecord', recordId],
    queryFn: () => salesRecordsApi.getById(recordId),
  });

  const mutation = useMutation({
    mutationFn: (input: UpdateSalesRecordInput) => salesRecordsApi.update(recordId, input),
    onSuccess: () => setSaved(true),
  });

  const handleFinish = (values: Record<string, unknown>) => {
    mutation.mutate({
      ...(values as Omit<UpdateSalesRecordInput, 'date'>),
      date: (values.date as dayjs.Dayjs).format('YYYY-MM-DD'),
    });
  };

  if (isLoading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', marginTop: 80 }}>
        <Spin size="large" />
      </div>
    );
  }

  if (error || !record) {
    return (
      <div style={{ maxWidth: 640, margin: '80px auto', padding: '0 16px' }}>
        <Alert type="error" title={error ? getErrorMessage(error) : 'Record not found.'} showIcon />
      </div>
    );
  }

  return (
    <div style={{ maxWidth: 640, margin: '40px auto', padding: '0 16px' }}>
      <Typography.Title level={3}>Edit record #{record.id}</Typography.Title>

      {saved ? (
        <>
          <Alert style={{ marginBottom: 16 }} type="success" title="Saved." showIcon />
          <Button onClick={() => window.close()}>Close tab</Button>
        </>
      ) : (
        <Form
          layout="vertical"
          initialValues={{ ...record, date: dayjs(record.date) }}
          onFinish={handleFinish}
        >
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item name="date" label="Date" rules={[{ required: true }]}>
                <DatePicker style={{ width: '100%' }} />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="companyName" label="Company" rules={[{ required: true }]}>
                <Input />
              </Form.Item>
            </Col>

            <Col span={12}>
              <Form.Item name="contactPerson" label="Contact person" rules={[{ required: true }]}>
                <Input />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="phone" label="Phone" rules={[{ required: true }]}>
                <Input />
              </Form.Item>
            </Col>

            <Col span={12}>
              <Form.Item name="email" label="Email" rules={[{ required: true }]}>
                <Input />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="region" label="Region" rules={[{ required: true }]}>
                <Input />
              </Form.Item>
            </Col>

            <Col span={12}>
              <Form.Item name="product" label="Product" rules={[{ required: true }]}>
                <Input />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="unitsSold" label="Units sold" rules={[{ required: true }]}>
                <InputNumber style={{ width: '100%' }} min={0} />
              </Form.Item>
            </Col>

            <Col span={12}>
              <Form.Item name="unitPrice" label="Unit price" rules={[{ required: true }]}>
                <InputNumber style={{ width: '100%' }} min={0} step={0.01} />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item name="revenue" label="Revenue" rules={[{ required: true }]}>
                <InputNumber style={{ width: '100%' }} min={0} step={0.01} />
              </Form.Item>
            </Col>
          </Row>

          {mutation.isError && (
            <Alert style={{ marginBottom: 16 }} type="error" title={getErrorMessage(mutation.error)} showIcon />
          )}

          <Button type="primary" htmlType="submit" loading={mutation.isPending} block>
            Save
          </Button>
        </Form>
      )}
    </div>
  );
}
