namespace SalesPlatform.Application.SalesReports.Queries.GetSalesReportList;

// Items already Skip/Take'd; TotalCount reflects the full filtered set (before paging) —
// exactly what AntD's Table.pagination needs (current page's rows + the grand total).
public record SalesReportsPage(IReadOnlyList<SalesReportListItem> Items, int TotalCount);
