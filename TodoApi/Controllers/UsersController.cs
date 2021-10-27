using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoApi.Authentication;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IJwtAuth _jwtAuth;
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

        public UsersController(IJwtAuth jwtAuth, IMyClaim myClaim,
            ILogger<UsersController> logger)
        {
            _jwtAuth = jwtAuth;
            _myClaim = myClaim;
            _logger = logger;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            var username = _myClaim.ParseAuthClaim(HttpContext);

            _logger.Log(LogLevel.Debug, $"User {username} is getting all users.");
            
            return _users;
        }

        // POST api/<UsersController>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserCredential userCredential)
        {
            var token = _jwtAuth.Authenticate(userCredential.UserName, userCredential.Password);
            if (token == null)
            {
                _logger.Log(LogLevel.Debug, $"User {userCredential.UserName} cannot be authenticated.");
                System.Diagnostics.Debug.WriteLine("TESt test test test ste");
                return Unauthorized();
            }
            _logger.Log(LogLevel.Information, $"User {userCredential.UserName} is authenticated.");
            return Ok(token);
        }
    }
}