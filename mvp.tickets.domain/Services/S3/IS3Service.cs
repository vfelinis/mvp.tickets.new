namespace mvp.tickets.domain.Services
{
    public interface IS3Service
    {
        Task<bool> PutObjectAsync(string fileName, Stream stream);
        Task<byte[]> GetObjectStreamAsync(string fileName);
        Task DeleteObjectAsync(string fileName);
        Task CopyObjectAsync(string sourceFileName, string destinationFileName);
    }
}