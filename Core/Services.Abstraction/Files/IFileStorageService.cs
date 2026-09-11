namespace LibraryManagement.Services.Abstraction.Files
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Stream fileStream, string extention, string folderName);
        void DeleteFile(string relativeFilePath);
    }
}
