using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataModel;
using Microsoft.Extensions.Primitives;

namespace VulcanWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // http://vulcanwebapi.azurewebsites.net/api/values
        [HttpGet]
        public APIResult Get()
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

        // http://vulcanwebapi.azurewebsites.net/api/values/QueryStringGet?id=777
        [HttpGet("QueryStringGet")]
        public APIResult QueryStringGet(int id)
        {
            APIResult foo;
            if (id == 777)
            {
                foo = new APIResult()
                {
                    Success = true,
                    Message = "透過 Get 方法，接收到 Id=777",
                    Payload = new APIData()
                    {
                        Id = 777,
                        Name = "Vulcan01"
                    }
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
        
        // http://vulcanwebapi.azurewebsites.net/api/values/777
        [HttpGet("{id}")]
        public APIResult Get(int id)
        {
            APIResult foo;
            if (id == 777)
            {
                foo = new APIResult()
                {
                    Success = true,
                    Message = "透過 Get 方法，接收到 Id=777",
                    Payload = new APIData()
                    {
                        Id = 777,
                        Name = "Vulcan01"
                    }
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

        // http://vulcanwebapi.azurewebsites.net/api/Values
        // 使用 JSON 格式
        [HttpPost]
        public APIResult Post([FromBody]APIData value)
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

        // http://vulcanwebapi.azurewebsites.net/api/Values/FormUrlencodedPost
        // 使用 FormUrlEncodedContent
        [HttpPost("FormUrlencodedPost")]
        public APIResult FormUrlencodedPost([FromForm]APIData value)
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

        [HttpPut]
        public APIResult Put(int id, [FromBody]APIData value)
        {
            APIResult foo;

            if (value.Id == 777)
            {
                foo = new APIResult()
                {
                    Success = true,
                    Message = "透過 Put 方法，接收到 Id=777 資料",
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

        [HttpDelete("{id}")]
        public APIResult Delete(int id)
        {
            APIResult foo;

            if (id == 777)
            {
                foo = new APIResult()
                {
                    Success = true,
                    Message = "Id=777 資料 已經刪除了",
                    Payload = null
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
    
        // http://vulcanwebapi.azurewebsites.net/api/values/HeaderPost
        [HttpPost("HeaderPost")]
        public APIResult HeaderGet([FromBody]LoginInformation loginInformation)
        {
            APIResult foo;
            StringValues VerifyCode = "";

            this.HttpContext.Request.Headers.TryGetValue("VerifyCode", out VerifyCode);
            if (StringValues.IsNullOrEmpty(VerifyCode))
            {
                foo = new APIResult()
                {
                    Success = false,
                    Message = "驗證碼沒有發現",
                    Payload = null
                };
            }
            else
            {
                if (VerifyCode != "123")
                {
                    foo = new APIResult()
                    {
                        Success = false,
                        Message = "驗證碼不正確",
                        Payload = null
                    };
                }
                else
                {
                    if (loginInformation.Account == "Vulcan" &&
                        loginInformation.Password == "123")
                    {
                        foo = new APIResult()
                        {
                            Success = true,
                            Message = "這個帳號與密碼正確無誤",
                            Payload = null
                        };
                    }
                    else
                    {
                        foo = new APIResult()
                        {
                            Success = false,
                            Message = "這個帳號與密碼不正確",
                            Payload = null
                        };
                    }
                }
            }
            return foo;
        }

    }
}
