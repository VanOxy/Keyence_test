// Mirrors the backend DTOs (SalesPlatform.Application) — keep in sync by hand for now,
// there's no shared codegen between the two projects.

export type UserRole = 'SalesRep' | 'Manager' | 'Admin';

export interface LoginResult {
  token: string;
  fullName: string;
  email: string;
  role: UserRole;
}

export interface RowError {
  rowNumber: number;
  column: string;
  value: unknown;
  message: string;
}

export interface ImportResult {
  success: boolean;
  totalRows: number;
  validRows: number;
  invalidRows: number;
  errors: RowError[];
  salesReportId: string | null;
}
