import { httpClient } from './httpClient';
import type { LoginResult } from './types';

export const authApi = {
  login: (email: string, password: string) =>
    httpClient.post<LoginResult>('/api/auth/login', { email, password }).then((res) => res.data),
};
