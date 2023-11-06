using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace WebApplication1.Controllers
{
    [Route("file")]
    [Authorize]
    public class FileController : ControllerBase
    {
        public ActionResult GetFile([FromQuery] string fileName)
        {
            var rootPath = Directory.GetCurrentDirectory();

            var filePath = $"{rootPath}/PrivateFiles/{fileName}";

            bool fileExists = System.IO.File.Exists(filePath);
            if(!fileExists)
            {
                return NotFound();
            }

            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(fileName, out string fileType);

            var fileContents = System.IO.File.ReadAllBytes(filePath);

            return File(fileContents, fileType, fileName);
        }
    }
}
