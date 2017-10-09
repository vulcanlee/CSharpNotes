using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataModel;

namespace VulcanWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class MyController : Controller
    {
        [HttpPost("FormUrlencodedPost")]
        public APIResult Post([FromForm]APIData value)
        {
            APIResult foo;

            if (value.Id == 777)
            {
                foo = new APIResult()
                {
                    Success = true,
                    Message = "透過 post 方法，接收到 Id=777 資料",
                    Payload = value
                };
            }
            else
            {
                foo = new APIResult()
                {
                    Success = false,
                    Message = "無法發現到指定的 ID",
                    Payload = null
                };
            }
            return foo;
        }

    }
}
