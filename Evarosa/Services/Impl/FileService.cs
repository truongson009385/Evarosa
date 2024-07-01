using Evarosa.Utils;

namespace Evarosa.Services.Impl
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private string _SOURCE;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
            _SOURCE = Path.Combine(_env.ContentRootPath, "Uploads");
        }

        public async Task<(string FilePath, string FileName)> UploadFileAsync(string folderName, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            CreateFolder(folderName);

            string fileName = GenerateUniqueFileName(file);
            string path = GetPath(folderName, fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return (path, fileName);
        }

        public string DeleteFile(string folderName, string fileName)
        {
            var path = GetPath(folderName, fileName);

            if (!Directory.Exists(path))
            {
                return $"{fileName} không tồn tại";
            }

            GetFile(folderName, fileName).Delete();

            return $"{fileName} đã xóa";
        }

        public void CreateFolder(string folderName)
        {
            var path = GetPath(_SOURCE, folderName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string DeleteFolder(string folderName)
        {
            var folderPath = Path.Combine(_SOURCE, folderName);

            if (!Directory.Exists(folderPath))
            {
                return $"{folderName} không tồn tại";
            }

            Directory.Delete(folderPath, recursive: true);
            return $"{folderName} đã xóa";
        }

        public FileInfo GetFile(string folderName, string fileName)
        {
            var path = GetPath(folderName, fileName);

            if (!Directory.Exists(path))
            {
                throw new Exception("Không tìm thấy file");
            }

            return new FileInfo(path);
        }

        public string GetPath(string folderName, string? fileName)
        {
            var path = Path.Combine(_SOURCE, folderName);

            if (!string.IsNullOrEmpty(fileName))
            {
                path = Path.Combine(path, fileName);
            }

            return path;
        }

        private string GenerateUniqueFileName(IFormFile file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            return $"{HtmlHelpers.ConvertToUnSign(fileName)}-{timestamp}{extension}";
        }
    }
}
