namespace SalesPlatform.Domain.Entities;

public class SalesRecord
{
    public long Id { get; set; }

    public Guid SalesReportId { get; set; }
    public Guid OwnerUserId { get; set; }

    public required DateOnly Date { get; set; }
    public required string CompanyName { get; set; }
    public required string ContactPerson { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required string Region { get; set; }
    public required string Product { get; set; }
    public required int UnitsSold { get; set; }
    public required decimal UnitPrice { get; set; }
    public required decimal Revenue { get; set; }
}