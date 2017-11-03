using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataModel;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using VulcanWebAPI.Filters;

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
                        Name = "Vulcan01",
                        Filename = DateTime.Now.ToString()
                    },
                    new APIData()
                    {
                        Id =234,
                        Name ="Vulcan02",
                        Filename = DateTime.Now.ToString()
                    }
                }
            };
            return foo;
        }

        [HttpGet("QueryStringGet")]
        public APIResult QueryStringGet([FromQuery] APIData value)
        {
            APIResult foo;
            if (value.Id == 777)
            {
                foo = new APIResult()
                {
                    Success = true,
                    Message = "透過 Get 方法，接收到 Id=777",
                    Payload = new APIData()
                    {
                        Id = 777,
                        Name = "Vulcan by QueryStringGet"
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

        [HttpGet("GetException")]
        public APIResult GetException()
        {
            APIResult foo = new APIResult();
            throw new Exception("喔喔,我發生錯誤了");
            return foo;
        }

        [HttpGet("GetExceptionFilter")]
        [CustomExceptionFilter]
        public APIResult GetExceptionFilter()
        {
            APIResult foo = new APIResult();
            throw new Exception("喔喔,我發生錯誤了");
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

        [HttpPost("Login")]
        public async Task<APIResult> Login([FromBody]LoginInformation loginInformation)
        {
            APIResult foo;

            if (loginInformation.Account == "Vulcan" &&
                loginInformation.Password == "123")
            {
                var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, "Herry"),
                new Claim(ClaimTypes.Role, "Users")
            };
                var claimsIdentity = new ClaimsIdentity(claims, "myTest");
                var principal = new ClaimsPrincipal(claimsIdentity);
                try
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties
                        {
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                            IsPersistent = true,
                            AllowRefresh = true
                        });
                    foo = new APIResult()
                    {
                        Success = true,
                        Message = "這個帳號與密碼正確無誤",
                        Payload = null
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    foo = new APIResult()
                    {
                        Success = false,
                        Message = "這個帳號與密碼不正確",
                        Payload = null
                    };
                }
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

            return foo;
        }

        [Authorize]
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

        [HttpGet("LongTimeGet")]
        public async Task<APIResult> LongTimeGet()
        {
            APIResult foo;

            await Task.Delay(5000);
            foo = new APIResult()
            {
                Success = true,
                Message = "透過 Get 方法",
                Payload = new APIData()
                {
                    Id = 777,
                    Name = "Vulcan01"
                }
            };
            return foo;
        }

        [HttpGet("CustHandler")]
        public APIResult CustHandler()
        {
            APIResult foo;
            StringValues VerifyCode = "";

            this.HttpContext.Request.Headers.TryGetValue("APIKey", out VerifyCode);
            if (StringValues.IsNullOrEmpty(VerifyCode))
            {
                foo = new APIResult()
                {
                    Success = false,
                    Message = "API 金鑰 沒有發現",
                    Payload = null
                };
                Request.HttpContext.Response.Headers.Add("APIKeyEcho", "No API Key");
            }
            else
            {
                if (VerifyCode != "123")
                {
                    foo = new APIResult()
                    {
                        Success = false,
                        Message = "API 金鑰 不正確",
                        Payload = null
                    };
                    Response.Headers.Add("APIKeyEcho", "API Key is incorrect");
                }
                else
                {
                    foo = new APIResult()
                    {
                        Success = true,
                        Message = "API 金鑰 正確無誤",
                        Payload = null
                    };
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(VerifyCode);
                    string echo = Convert.ToBase64String(bytes);
                    Request.HttpContext.Response.Headers.Add("APIKeyEcho", echo);
                }
            }
            return foo;
        }
    }
}
