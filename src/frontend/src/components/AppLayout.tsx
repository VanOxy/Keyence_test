import { Layout } from 'antd';
import { Outlet } from 'react-router-dom';
import { AppHeader } from './AppHeader';

// Shared shell for every protected page: header on top, page content below via <Outlet/>.
// Wrapping the parent route once (see App.tsx) means new pages just add a nested <Route> —
// no need to repeat the header on each one.
export function AppLayout() {
  return (
    <Layout style={{ minHeight: '100vh' }}>
      <AppHeader />
      <Layout.Content>
        <Outlet />
      </Layout.Content>
    </Layout>
  );
}
