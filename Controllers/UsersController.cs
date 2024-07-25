using Microsoft.AspNetCore.Mvc;
using BankApi.EntityFramework;
using BankApi.Models.DTO;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.Intrinsics.Arm;
using Microsoft.AspNetCore.Components.Forms;
using System.Security.Cryptography;
using System.Reflection.Metadata;

namespace BankApi.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _userDbContext;
        private readonly IConfiguration configuration;
        public UsersController(UserDbContext userDbContext, IConfiguration config)
        {
            this._userDbContext = userDbContext;
            this.configuration = config;
        }

        [HttpPost]
        [Route("registration")]
        public IActionResult Registration(UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var objUser = _userDbContext.Users.FirstOrDefault(x => x.Email == userDTO.Email);
            if (objUser == null)
            {
                _userDbContext.Users.Add(new Models.UserModel
                {
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    Email = userDTO.Email,
                    Password = userDTO.Password
                });
                _userDbContext.SaveChanges();
                return Ok("Success Register");
            }
            else
            {
                return BadRequest("User already exists with the same email");
            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            var user = _userDbContext.Users.FirstOrDefault(x => x.Email == loginDTO.Email && x.Password == loginDTO.Password);
            if (user != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(user.Password);
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(bytes);
                    string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                    var claims = new[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Surname, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Email, user.Email.ToString()),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("Password", hashString)
                };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        configuration["Jwt:Issuer"],
                        configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: signIn
                        );
                    string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(new { Token = tokenValue });
                }
               
            }

            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        [Route("GetUsers")]
        public IActionResult GetUsers()
        {
            return Ok(_userDbContext.Users.ToList());
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        [Route("GetUser/{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _userDbContext.Users.FirstOrDefault(x => x.UserId == id);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NoContent();
            }
        }
    }
}
