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

        public async Task WriteAllTextAsync(string filePath, string content)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, content);
        }
    }
}
