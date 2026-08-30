import { AxiosError } from 'axios';
import type { ProblemDetails } from './types';

// Every backend error (ExceptionHandlingMiddleware) comes back as ProblemDetails —
// this pulls a single human-readable message out of it, whatever shape it took.
export function getErrorMessage(error: unknown): string {
  if (error instanceof AxiosError) {
    const problem = error.response?.data as ProblemDetails | undefined;
    if (problem?.errors) return Object.values(problem.errors).flat().join(' ');
    if (problem?.detail) return problem.detail;
    if (problem?.title) return problem.title;
  }
  return 'Something went wrong. Please try again.';
}
