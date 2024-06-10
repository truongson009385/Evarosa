using Microsoft.Extensions.FileProviders;

namespace Evarosa.Services
{
    public interface IFileService
    {
        public Task<(string FilePath, string FileName)> UploadFileAsync(string folderName, IFormFile file);
        public string DeleteFile(string folderName, string fileName);
        public void CreateFolder(string folderName);
        public string DeleteFolder(string folderName);
        public FileInfo GetFile(string folderName, string fileName);
        public string GetPath(string folderName, string? fileName);
    }
}
