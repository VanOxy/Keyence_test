import axios from 'axios';
import { notifyUnauthorized } from './authEvents';

// The only place in the app that talks HTTP directly — every feature calls
// through a dedicated api/*.ts module built on top of this instance, never fetch/axios directly.
export const httpClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

httpClient.interceptors.request.use((config) => {
  const stored = localStorage.getItem('auth');
  const token = stored ? (JSON.parse(stored).token as string) : null;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// An expired/rejected token ends the session — AuthProvider logs out, ProtectedRoute redirects.
// The error still propagates so the calling code can render it if it wants to.
httpClient.interceptors.response.use(undefined, (error) => {
  if (error.response?.status === 401) {
    notifyUnauthorized();
  }
  return Promise.reject(error);
});
