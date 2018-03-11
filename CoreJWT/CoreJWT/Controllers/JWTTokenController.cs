using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreJWT.Controllers
{
    [Produces("application/json")]
    [Route("api/JWTToken")]
    public class JWTTokenController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "JWTToken", "Get ( no Authorize )" };
        }

        [HttpPut]
        [Authorize]
        public IEnumerable<string> Put()
        {
            return new string[] { "JWTToken", "Put ( has Authorize)" };
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IEnumerable<string> Post()
        {
            return new string[] { "JWTToken", "Post ( has Authorize and Roles=Admin )" };
        }

        [HttpDelete]
        [Authorize(Roles = "Admini")]
        public IEnumerable<string> Delete()
        {
            return new string[] { "JWTToken", "Post ( has Authorize and Roles=Admini )" };
        }
    }
}
