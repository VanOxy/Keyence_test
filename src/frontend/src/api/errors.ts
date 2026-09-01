import { AxiosError } from 'axios';
import { ClientError } from 'graphql-request';
import type { ProblemDetails } from './types';

export function getErrorMessage(error: unknown): string {
  if (error instanceof AxiosError) {
    const problem = error.response?.data as ProblemDetails | undefined;
    if (problem?.errors) return Object.values(problem.errors).flat().join(' ');
    if (problem?.detail) return problem.detail;
    if (problem?.title) return problem.title;
  }
  if (error instanceof ClientError) {
    const message = error.response.errors?.[0]?.message;
    if (message) return message;
  }
  return 'Something went wrong. Please try again.';
}
