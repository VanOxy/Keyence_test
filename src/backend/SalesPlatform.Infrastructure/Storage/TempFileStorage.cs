using SalesPlatform.Application.Common.Interfaces;

namespace SalesPlatform.Infrastructure.Storage;

public class TempFileStorage : IFileStorage
{
    private static readonly string UploadsDirectory = Path.Combine(Path.GetTempPath(), "SalesPlatformUploads");

    public async Task<string> SaveAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(UploadsDirectory);

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var filePath = Path.Combine(UploadsDirectory, uniqueFileName);

        await using var destination = File.Create(filePath);
        await fileStream.CopyToAsync(destination, cancellationToken);

        return filePath;
    }

    public void Delete(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
