import { Button, Layout, Menu, Space, Typography } from 'antd';
import { useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import './AppHeader.css';

const navItems = [
  { key: '/upload', label: 'Upload' },
  { key: '/reports', label: 'Reports' },
];

export function AppHeader() {
  const { user, logout } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Layout.Header style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
      <div style={{ display: 'flex', alignItems: 'center', gap: 32 }}>
        <Typography.Text strong style={{ color: '#fff', fontSize: 18 }}>
          SalesPlatform
        </Typography.Text>
        <Menu
          theme="dark"
          mode="horizontal"
          selectedKeys={[location.pathname]}
          items={navItems}
          onClick={({ key }) => navigate(key)}
          style={{ minWidth: 200, borderBottom: 'none' }}
        />
      </div>

      <Space size="middle">
        {user && (
          <Typography.Text style={{ color: 'rgba(255, 255, 255, 0.85)' }}>
            {user.fullName} · {user.role}
          </Typography.Text>
        )}
        <Button onClick={handleLogout}>Log out</Button>
      </Space>
    </Layout.Header>
  );
}
