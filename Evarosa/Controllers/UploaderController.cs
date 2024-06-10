using Evarosa.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evarosa.Controllers
{
    [Authorize(AuthenticationSchemes = "vcms")]
    public class UploaderController(IFileService fileService) : Controller
    {
        [HttpPost]
        [Route("/uploads/{folderName}")]
        [RequestSizeLimit(104857600)] // 100 MB
        public async Task<IActionResult> Upload(string folderName = "project")
        {
            try
            {
                var files = Request.Form.Files;
                var arrString = new List<string>();

                foreach (var item in files)
                {
                    var fileStr = await fileService.UploadFileAsync(folderName, item);

                    arrString.Add(fileStr.FileName);
                }

                return Json(new { success = true, files = arrString });
            }
            catch (Exception ex)
            {
                var msg = new { success = false, msg = ex.Message };
                return Json(msg);
            }
        }

        [HttpDelete("/delete/{folderName}/{filename}")]
        public IActionResult DeleteFile(string folderName, string fileName)
        {
            try
            {
                var file = fileService.DeleteFile(folderName, fileName);

                if (!string.IsNullOrEmpty(file))
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
