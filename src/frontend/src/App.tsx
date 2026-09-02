import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { AppLayout } from './components/AppLayout';
import { AuthProvider } from './auth/AuthContext';
import { ProtectedRoute } from './auth/ProtectedRoute';
import { LoginPage } from './pages/LoginPage';
import { UploadPage } from './pages/UploadPage';
import { SalesReportsListPage } from './pages/SalesReportsListPage';
import { SalesRecordsListPage } from './pages/SalesRecordsListPage';
import { SalesRecordEditPage } from './pages/SalesRecordEditPage';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route
            element={
              <ProtectedRoute>
                <AppLayout />
              </ProtectedRoute>
            }
          >
            <Route path="/upload" element={<UploadPage />} />
            <Route path="/reports" element={<SalesReportsListPage />} />
            <Route path="/records" element={<SalesRecordsListPage />} />
          </Route>
          {/* Opens in a new tab from the records table — no header/nav, just the form. */}
          <Route
            path="/records/:id/edit"
            element={
              <ProtectedRoute>
                <SalesRecordEditPage />
              </ProtectedRoute>
            }
          />
          <Route path="*" element={<Navigate to="/upload" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
