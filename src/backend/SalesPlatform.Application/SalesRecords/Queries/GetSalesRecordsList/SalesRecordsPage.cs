namespace SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordsList;

// Items already Skip/Take'd; TotalCount reflects the full filtered set (before paging) —
// exactly what AntD's Table.pagination needs.
public record SalesRecordsPage(IReadOnlyList<SalesRecordListItem> Items, int TotalCount);
