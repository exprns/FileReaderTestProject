

using System.Threading.Tasks;
using FileReader.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileReader.Controllers
{
    [Route("file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private IFileService fileService;
        public FileController(IFileService fileService)
        {
            this.fileService = fileService;
        }

        /// <summary>
        /// Get txt file by filename.
        /// </summary>
        /// <param name="filename"> file name for download. </param>
        /// <returns> Text file</returns>
        /// <remarks> If filename is not correct will return null. </remarks> 
        [Produces("application/txt")]
        [HttpGet("filename={filename}")]
        public async Task<IActionResult> DownloadTxtFile(string filename)
        {
            // TODO: понять что такое нагрузочное тестирование
            // TODO: понять делать нагрузочные тесты на запросы в контроллере или же просто методы (хотя методы это юнит тестами называлось)
            // TODO: понять как сделать метрики
            if (filename == null)
                return BadRequest();

            var data = await fileService.GetFile(filename);

            if (data == null)
                return BadRequest();

            return File(data, "text/plain", filename);
        }
    }
}