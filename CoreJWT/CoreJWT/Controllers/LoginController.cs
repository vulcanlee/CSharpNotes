using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoreJWT.Controllers
{
    [Produces("application/json")]
    [Route("api/Login")]
    public class LoginController : Controller
    {
        public IConfiguration Configuration { get; }
        public LoginController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, "vulcan.lee@vulcan.net"),
                new Claim(ClaimTypes.Role, "Admini"),
            };

            var token = new JwtSecurityToken
            (
                issuer: Configuration["Tokens:ValidIssuer"],
                audience: Configuration["Tokens:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(Configuration["Tokens:IssuerSigningKey"])),
                SecurityAlgorithms.HmacSha256)
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        // POST: api/Login
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var claims = new[]
      {
                new Claim(JwtRegisteredClaimNames.NameId, "vulcan.lee@vulcan.net"),
                new Claim(ClaimTypes.Role, "Admini"),
            };

            var token = new JwtSecurityToken
            (
                issuer: Configuration["Tokens:ValidIssuer"],
                audience: Configuration["Tokens:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey
                            (Encoding.UTF8.GetBytes(Configuration["Tokens:IssuerSigningKey"])),
                        SecurityAlgorithms.HmacSha256)
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
