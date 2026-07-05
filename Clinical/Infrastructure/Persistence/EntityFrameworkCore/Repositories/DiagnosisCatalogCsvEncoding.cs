using System.Text;

namespace VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public static class DiagnosisCatalogCsvEncoding
{
    public static async Task<StringReader> CreateReaderAsync(
        Stream csvStream,
        CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        await csvStream.CopyToAsync(memoryStream, cancellationToken);

        return new StringReader(Decode(memoryStream.ToArray()));
    }

    public static async Task<StringReader> CreateReaderAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
        return new StringReader(Decode(bytes));
    }

    private static string Decode(byte[] bytes)
    {
        return Encoding.Latin1.GetString(bytes);
    }
}
