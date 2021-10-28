using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoApi.Authentication;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Authorize(AuthenticationSchemes = GeneralAuth.AuthSchemes)]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IJwtAuth _jwtAuth;
        private readonly ICookieAuth _cookieAuth;
        private readonly IMyClaim _myClaim;
        private readonly ILogger _logger;

        private readonly List<User> _users = new List<User>()
        {
            new User
                {
                    Id=1, Name="tester" 
                },
            new User
                {
                    Id=2, Name="tester2"
                }
        };

        public UsersController(IJwtAuth jwtAuth, ICookieAuth cookieAuth,
            IMyClaim myClaim, ILogger<UsersController> logger)
        {
            _jwtAuth = jwtAuth;
            _cookieAuth = cookieAuth;
            _myClaim = myClaim;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            var username = _myClaim.ParseAuthClaim(HttpContext);

            _logger.Log(LogLevel.Debug, $"User {username} is getting all users.");
            
            return _users;
        }

        // POST api/Users/authenticate/jwt
        [AllowAnonymous]
        [HttpPost("authenticate/jwt")]
        public IActionResult AuthenticateAsJwt([FromBody]UserCredential userCredential)
        {
            var token = _jwtAuth.Authenticate(userCredential.UserName, userCredential.Password);
            if (token == null)
            {
                _logger.Log(LogLevel.Debug, $"User {userCredential.UserName} cannot be authenticated.");
                return Unauthorized();
            }
            _logger.Log(LogLevel.Information, $"User {userCredential.UserName} is authenticated.");
            return Ok(token);
        }

        // POST api/Users/authenticate/cookie
        [AllowAnonymous]
        [HttpPost("authenticate/cookie")]
        public async Task<IActionResult> AuthenticateAsCookieAsync([FromBody]UserCredential userCredential)
        {
            var result = await _cookieAuth.AuthenticateAsync(userCredential.UserName, userCredential.Password, HttpContext);
            if (!result)
            {
                _logger.Log(LogLevel.Debug, $"User {userCredential.UserName} cannot be authenticated.");
                return Unauthorized();
            }
            _logger.Log(LogLevel.Information, $"User {userCredential.UserName} is authenticated.");
            return NoContent();
        }

        // POST api/Users/signout/cookie
        [HttpPost("signout/cookie")]
        public async Task<IActionResult> SignOutAsCookieAsync()
        {
            var username = _myClaim.ParseAuthClaim(HttpContext);

            _logger.Log(LogLevel.Information, $"User {username} is signing out.");
            await _cookieAuth.SignOutAsync(HttpContext);
            return NoContent();
        }
    }
}