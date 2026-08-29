import axios from 'axios';

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
