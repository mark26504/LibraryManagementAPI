namespace LibraryManagement.Persistence.Files
{
    public class FileStorageOptions
    {
        public const string SectionName = "FileStorage";

        public string[] AllowedFolders { get; set; } = { "uploads/books" };

        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".webp" };
    }
}
