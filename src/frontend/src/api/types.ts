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

export interface SalesReportListItem {
  id: string;
  originalFileName: string;
  createdAtUtc: string;
}

export interface SalesRecordListItem {
  id: number;
  date: string;
  companyName: string;
  contactPerson: string;
  phone: string;
  email: string;
  region: string;
  product: string;
  unitsSold: number;
  unitPrice: number;
  revenue: number;
}

// Shape of every error response from ExceptionHandlingMiddleware (RFC 7807).
export interface ProblemDetails {
  title: string;
  status: number;
  detail?: string;
  // Only present for 400s from FluentValidation: field name -> messages.
  errors?: Record<string, string[]>;
}
