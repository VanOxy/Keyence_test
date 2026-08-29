namespace SalesPlatform.Application.Common.Interfaces;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    void Delete(string filePath);
}
