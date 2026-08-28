using SalesPlatform.Domain.Enums;

namespace SalesPlatform.Domain.Entities;

public class UploadBatch
{
    public Guid Id { get; set; }
    public Guid UploadedByUserId { get; set; }
    public required string OriginalFileName { get; set; }
    public BatchStatus Status { get; set; } = BatchStatus.Pending;
    public string? ErrorsJson { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
}