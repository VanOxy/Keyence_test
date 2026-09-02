import { ClientError, GraphQLClient } from 'graphql-request';
import { notifyUnauthorized } from './authEvents';

// A rejected token reaches us in one of two shapes depending on how HotChocolate negotiates the
// response: HTTP 401, or HTTP 200 with an AUTH_NOT_AUTHENTICATED error in the body. Both mean the
// session is over. A role denial (AUTH_NOT_AUTHORIZED) is deliberately NOT included — being logged
// in but lacking permission shouldn't kick anyone back to the login page.
function isSessionExpired(payload: unknown): boolean {
  const { status, errors } = (payload ?? {}) as {
    status?: number;
    errors?: readonly { extensions?: { code?: unknown } }[];
  };

  return status === 401 || (errors?.some((error) => error.extensions?.code === 'AUTH_NOT_AUTHENTICATED') ?? false);
}

// GraphQL counterpart to httpClient.ts — same auth token, same base host, different transport.
// Every feature that talks GraphQL goes through this, never a raw GraphQLClient/fetch.
export const gqlClient = new GraphQLClient(`${import.meta.env.VITE_API_URL}/graphql`, {
  headers: () => {
    const stored = localStorage.getItem('auth');
    const token = stored ? (JSON.parse(stored).token as string) : null;
    return token ? { Authorization: `Bearer ${token}` } : ({} as Record<string, string>);
  },
  responseMiddleware: (response) => {
    if (isSessionExpired(response instanceof ClientError ? response.response : response)) {
      notifyUnauthorized();
    }
  },
});
