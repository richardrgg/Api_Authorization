using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api_Authorization.Models;
using Api_Authorization.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api_Authorization.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ApiContext _context;

        public AccountController(
            IUserService userService, 
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            ApiContext context)
        {
            _userService = userService;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _context = context;

            _userService.CreateUserDemo();
        }

        [HttpPost]
        [Route("singup")]
        public async Task<IActionResult> Signup([FromBody] User model)
        {
            var user = _userService.ValidateUsername(model.Username);
            if (user != null) return BadRequest();
            model.Password = _passwordHasher.GenerateIdentityV3Hash(model.Password);
            _userService.Create(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = _userService.ValidateUsername(model.Username);
            if (user is null) return BadRequest();

            //if (user.Password.Equals(model.Password) == false) return BadRequest();
            if (_passwordHasher.VerifyIdentityV3Hash(model.Password, user.Password) == false) return BadRequest();

            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var jwt = _tokenService.GenerateToken(userClaims);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            await _context.SaveChangesAsync();

            return new ObjectResult(new { token = jwt, refreshToken = refreshToken });
        }

        //[Authorize(Roles = "Admin123")]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            return Content($"The user: {User.Identity.Name} made an authenticated call at {DateTime.Now.ToString("HH:mm:ss")}", "text/plain");
        }



    }
}