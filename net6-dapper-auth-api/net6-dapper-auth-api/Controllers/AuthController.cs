using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repository;
using static Repository.AuthRepository;

namespace net6_dapper_auth_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _user;

        public AuthController(IAuthRepository user)
        {
            _user = user;
        }

        [HttpPost("signup")]
        public ActionResult<int> SignUp([FromBody] IAuth user)
        {
            if (user.Username == null || user.Password == null)
                return BadRequest(new { message = "Username or password if not null" });

            return _user.SignUp(user.Username, user.Password);
        }

        [HttpPost("signin")]
        [ProducesResponseType(200, Type = typeof(JwtSecurityTokenHandler))]
        [ProducesResponseType(400)]
        public IActionResult SignIn([FromBody] IAuth userParam)
        {
            var user = _user.SignIn(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });


            var userRole = "Admin";

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySecretKey010203"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                claims: new List<Claim> {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, userRole)
                },
                expires: DateTime.Now.AddDays(2),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return Ok(new { Token = tokenString });

        }

        [HttpGet("gethash/{value}")]
        public ActionResult<string> GetHash(string value)
        {
            return BCrypt.Net.BCrypt.HashPassword(value);
        }

        
    }
}