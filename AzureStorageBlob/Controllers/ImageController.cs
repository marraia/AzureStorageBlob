using System.Threading.Tasks;
using AzureStorageBlob.Helpers;
using AzureStorageBlob.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureStorageBlob.Controllers
{
    [Produces("application/json")]
    [Route("api/Image")]
    public class ImageController : Controller
    {
        private readonly StorageConfig _storageConfig = null;

        public ImageController(IOptions<StorageConfig> config)
        {
            _storageConfig = config.Value;
        }

        [HttpPost]
        public async Task<IActionResult> PostImage(IFormFile file)
        {
            var upload = new StorageHelper(_storageConfig);
            var format = file.FileName.Trim('\"');

            if (upload.IsImage(format))
            {
                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                        return Ok(await upload.Upload(stream, file.FileName));
                }
            }
            else
            {
                return new UnsupportedMediaTypeResult();
            }

            return BadRequest("Erro");
        }
    }
}