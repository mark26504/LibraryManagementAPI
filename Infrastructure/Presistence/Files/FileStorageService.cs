namespace LibraryManagement.Persistence.Files
{
    internal sealed class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly FileStorageOptions _options;

        public FileStorageService(
            IWebHostEnvironment environment,
            IOptions<FileStorageOptions> options)
        {
            _environment = environment;
            _options = options.Value;
        }


        public async Task<string> SaveFileAsync(Stream fileStream, string extension, string folderName)
        {
            var normalizedExtension = NormalizeExtension(extension);
            var normalizedFolder = NormalizeFolder(folderName);

            var fileName = $"{Guid.NewGuid()}{normalizedExtension}";

            var folderPath = Path.Combine(_environment.WebRootPath, normalizedFolder);
            Directory.CreateDirectory(folderPath);

            var physicalFilePath = Path.Combine(folderPath, fileName);

            await using var destination = new FileStream(physicalFilePath, FileMode.Create);
            await fileStream.CopyToAsync(destination);

            return $"/{normalizedFolder}/{fileName}";
        }

        public void DeleteFile(string relativeFilePath)
        {
            if (string.IsNullOrWhiteSpace(relativeFilePath))
                return;

            var relative = relativeFilePath.TrimStart('/');
            var filePath = Path.Combine(_environment.WebRootPath, relative);

            if (!IsInsideWebRoot(filePath))
                return;

            if (!File.Exists(filePath))
                return;

            File.Delete(filePath);
        }

        #region Helper Methods

        private string NormalizeExtension(string extension)
        {
            var normalized = extension?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(normalized))
                throw new ArgumentException("File extension is required.", nameof(extension));

            if (!normalized.StartsWith('.'))
                normalized = "." + normalized;

            if (!_options.AllowedExtensions.Contains(normalized))
                throw new ArgumentException($"File extension {normalized} is not allowed.", nameof(extension));

            return normalized;
        }

        private string NormalizeFolder(string folderName)
        {
            var normalized = folderName?.Trim().TrimStart('/').Replace('\\', '/').ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(normalized) || !_options.AllowedFolders.Contains(normalized))
                throw new ArgumentException($"Upload folder {folderName} is not allowed.", nameof(folderName));

            return normalized;
        }

        private bool IsInsideWebRoot(string filePath)
        {
            var root = Path.GetFullPath(_environment.WebRootPath);
            var full = Path.GetFullPath(filePath);

            return full.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

    }
}
