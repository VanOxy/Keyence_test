import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import { message } from 'antd';
import { setUnauthorizedHandler } from '../api/authEvents';
import type { LoginResult } from '../api/types';

interface AuthContextValue {
  user: LoginResult | null;
  login: (result: LoginResult) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function readStoredUser(): LoginResult | null {
  const stored = localStorage.getItem('auth');
  return stored ? (JSON.parse(stored) as LoginResult) : null;
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<LoginResult | null>(readStoredUser);

  const login = (result: LoginResult) => {
    localStorage.setItem('auth', JSON.stringify(result));
    setUser(result);
  };

  const logout = () => {
    localStorage.removeItem('auth');
    setUser(null);
  };

  // The server rejected our token (expired or otherwise invalid): drop the session and let
  // ProtectedRoute do the redirecting. A page usually fires several requests at once, so the
  // toast gets a fixed key — one message, not one per failed request.
  useEffect(() => {
    setUnauthorizedHandler(() => {
      logout();
      message.warning({ key: 'session-expired', content: 'Session expired — please log in again.' });
    });
  }, []);

  return <AuthContext.Provider value={{ user, login, logout }}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
