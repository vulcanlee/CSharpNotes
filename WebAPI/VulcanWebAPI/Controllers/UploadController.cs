using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using DataModel;

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

            string webDatasRoot = Path.Combine(_HostingEnvironment.WebRootPath, "Datas");

            long size = files.Sum(f => f.Length);

            // full path to file in temp location

            if (files.Count > 0)
            {
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.Combine(webDatasRoot, formFile.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        fooAPIResult.Success = true;
                        fooAPIResult.Message = "檔案上傳成功";
                        fooAPIResult.Payload = new APIData
                        {
                            Id = 3000,
                            Name = "Your Name",
                            Filename = formFile.FileName
                        };
                    }
                }
            }
            else
            {
                fooAPIResult.Success = false;
                fooAPIResult.Message = "沒有任何檔案上傳";
                fooAPIResult.Payload = null;
            }

            return fooAPIResult;
        }
    }
}
