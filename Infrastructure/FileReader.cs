namespace Infrastructure
{
    public class FileReader : IFileReader
    {
        public async Task<bool> ExistsAsync(string filePath)
        {
            return await Task.FromResult(File.Exists(filePath));
        }

        public async Task<string> ReadAllTextAsync(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }
    }
}
