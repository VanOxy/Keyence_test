namespace SalesPlatform.Domain.Entities;

// It is created only upon successful import (all file rows are valid).
// If the import fails, no record is added to the database at all; the error appears
// only in the response to the upload request itself (ImportResult).
public class SalesReport
{
    public Guid Id { get; set; }
    public Guid OwnerUserId { get; set; }
    public required string OriginalFileName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}