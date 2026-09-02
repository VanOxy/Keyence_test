import { Button, Layout, Space, Typography } from 'antd';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';

export function AppHeader() {
  const { user, logout } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();

  const isManagerOrAdmin = user?.role === 'Manager' || user?.role === 'Admin';
  const navItems = [
    { key: '/upload', label: 'Upload' },
    { key: '/reports', label: 'Reports' },
    ...(isManagerOrAdmin ? [{ key: '/records', label: 'Records' }] : []),
  ];

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Layout.Header style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
      <Space size={48} align="center">
        <Typography.Text strong style={{ color: '#fff', fontSize: 18 }}>
          SalesPlatform
        </Typography.Text>

        <Space size={24}>
          {navItems.map((item) => (
            <Link
              key={item.key}
              to={item.key}
              style={{ color: location.pathname === item.key ? '#fff' : 'rgba(255, 255, 255, 0.65)' }}
            >
              {item.label}
            </Link>
          ))}
        </Space>
      </Space>

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
