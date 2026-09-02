using LibraryManagement.Services.Abstraction.Contracts.Common;
using Microsoft.AspNetCore.Hosting;

namespace LibraryManagement.Services.Implementations.Common
{
    internal sealed class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string extention, string folderName)
        {
            //var fileName = Path.Combine(Guid.NewGuid().ToString(), extention); // Wrong -> Guid\.jpg

            var fileName = $"{Guid.NewGuid()}{extention}"; // Correct -> Guid.jpg

            var folderPath = Path.Combine(_environment.WebRootPath, folderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            
            var physicalFilePath = Path.Combine(folderPath, fileName);

            await using var stream = new FileStream(physicalFilePath, FileMode.Create);
            await fileStream.CopyToAsync(stream);

            return Path.Combine(folderName, fileName).Replace("\\", "/");
        }

        public void DeleteFile(string relativeFilePath)
        {
            var filePath = Path.Combine(_environment.WebRootPath, relativeFilePath);
            if (!File.Exists(filePath))
                return;
            File.Delete(filePath);
        }

    }
}
