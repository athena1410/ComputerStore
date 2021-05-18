using ComputerStore.Structure.Helper;
using ComputerStore.Structure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ComputerStore.Api.v2.Controllers
{
    [ApiVersion("2")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("Upload")]
        public IActionResult Upload()
        {
            var listPathResponse = new List<string>();

            var files = Request.Form.Files;
            if (files.Any(f => f.Length == 0))
            {
                return BadRequest();
            }

            var tempFolder = FileHelper.CreateTempFolder();
            foreach (var file in files)
            {
                var newFileName = FileHelper.GenerateFileName(file.FileName);                
                var filePath = Path.Combine(tempFolder, newFileName);

                listPathResponse.Add(filePath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            return Ok(new ApiResponse<List<string>>(listPathResponse));
        }
    }
}
