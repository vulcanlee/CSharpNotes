using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VulcanWebAPI.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VulcanWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        APIResult fooAPIResult = new APIResult();
        private IHostingEnvironment _HostingEnvironment;

        public UploadController(IHostingEnvironment hostingEnvironment)
        {
            _HostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<APIResult> Post(List<IFormFile> files)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads

            string fileName = "";  // 產生在 Server 上的圖片檔案名稱(將會配置 Guid
            string path = "";
            string webDatasRoot = Path.Combine(_HostingEnvironment.WebRootPath, "Datas");

            long size = files.Sum(f => f.Length);

            // full path to file in temp location

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(webDatasRoot, formFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            fooAPIResult.Success = false;
            fooAPIResult.Message = "沒有任何檔案上傳";
            fooAPIResult.Payload = null;

            return fooAPIResult;
        }
    }
}
