using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataModel;
using Microsoft.AspNetCore.Authorization;

namespace VulcanWebAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MyController : Controller
    {
        [HttpGet("LoginCheck")]
        public APIResult LoginCheck()
        {
            APIResult foo = new APIResult()
            {
                Success = true,
                Message = "成功得所有資料集合",
                Payload = new List<APIData>()
                {
                    new APIData()
                    {
                        Id =777,
                        Name = "Vulcan01"
                    },
                    new APIData()
                    {
                        Id =234,
                        Name ="Vulcan02"
                    }
                }
            };
            return foo;
        }
    }
}
