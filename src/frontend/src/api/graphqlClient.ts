import { GraphQLClient } from 'graphql-request';

// GraphQL counterpart to httpClient.ts — same auth token, same base host, different transport.
// Every feature that talks GraphQL goes through this, never a raw GraphQLClient/fetch.
export const gqlClient = new GraphQLClient(`${import.meta.env.VITE_API_URL}/graphql`, {
  headers: () => {
    const stored = localStorage.getItem('auth');
    const token = stored ? (JSON.parse(stored).token as string) : null;
    return token ? { Authorization: `Bearer ${token}` } : ({} as Record<string, string>);
  },
});
