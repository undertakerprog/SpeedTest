namespace Infrastructure
{
    public interface IFileReader
    {
        Task<bool> ExistsAsync(string filePath);
        Task<string> ReadAllTextAsync(string filePath);
    }
}
