using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using TodoApi.Authentication;
using TodoApi.Models;
using TodoApi.Identity;
using Microsoft.AspNetCore.Identity;

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
        private readonly IUserManagerWrapper _userManagerWrapper;
        private readonly IRoleManagerWrapper _roleManagerWrapper;

        public UsersController(IJwtAuth jwtAuth, ICookieAuth cookieAuth,
            IMyClaim myClaim, ILogger<UsersController> logger,
            IUserManagerWrapper userManagerWrapper, IRoleManagerWrapper roleManagerWrapper)
        {
            _jwtAuth = jwtAuth;
            _cookieAuth = cookieAuth;
            _myClaim = myClaim;
            _logger = logger;
            _userManagerWrapper = userManagerWrapper;
            _roleManagerWrapper = roleManagerWrapper;
        }

        // POST: api/Users
        [AllowAnonymous]
        [HttpPost]
        [ActionName("CreateAsync")]
        public async Task<ActionResult<User>> CreateAsync([FromBody] UserRegister userRegister)
        {
            var appUser = await _userManagerWrapper.FindByNameAsync(userRegister.UserName);
            if (appUser != null)
            {
                _logger.Log(LogLevel.Debug, $"Cannot create new user since it already exists.");
                return Conflict();
            }

            appUser = new AppUser()
            {
                Email = userRegister.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = userRegister.UserName
            };

            var result = await _userManagerWrapper.CreateAsync(appUser, userRegister.Password);
            if (!result.Succeeded)
            {
                _logger.Log(LogLevel.Error, $"Cannot create new user.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Message = "Cannot create the new user." });
            }

            appUser = await _userManagerWrapper.FindByNameAsync(appUser.UserName);
            if (appUser == null)
            {
                _logger.Log(LogLevel.Error, $"The newly created user still does not exist.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Message = "The newly created user still does not exist." });
            }

            // TODO: This available role creation should be moved to other places. This should not be tied to user creatoin.
            // Create the available roles if they do not exist yet
            if (!await _roleManagerWrapper.RoleExistsAsync(UserRole.Roles.Admin.ToString()))
            {
                await _roleManagerWrapper.CreateAsync(UserRole.Roles.Admin.ToString());
            }
            if (!await _roleManagerWrapper.RoleExistsAsync(UserRole.Roles.User.ToString()))
            {
                await _roleManagerWrapper.CreateAsync(UserRole.Roles.User.ToString());
            }

            if (await _roleManagerWrapper.RoleExistsAsync(userRegister.Role.ToString()))
            {
                result = await _userManagerWrapper.AddToRoleAsync(appUser, userRegister.Role.ToString());
                if (!result.Succeeded)
                {
                    _logger.Log(LogLevel.Warning, $"Fails to add the user to role {userRegister.Role.ToString()}.");
                }
            }

            return CreatedAtAction(nameof(CreateAsync), new User
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                Email = appUser.Email
            });
        }

        // DELETE: api/Users/5
        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var parsedClaim = _myClaim.ParseAuthClaim(HttpContext);

            var user = await _userManagerWrapper.FindByIdAsync(id);
            if (user == null)
            {
                _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} tries to delete a user with id {id} that does not exist.");
                return NotFound();
            }

            IdentityResult result;
            var roleList = await _userManagerWrapper.GetRolesAsync(user);
            foreach (var role in roleList)
            {
                result = await _userManagerWrapper.RemoveFromRoleAsync(user, role);
                if (!result.Succeeded)
                {
                    _logger.Log(LogLevel.Warning, $"User {parsedClaim.UserName} fails to remove the user with id {id} from role {role}.");
                }
            }

            result = await _userManagerWrapper.DeleteAsync(user);
            if (!result.Succeeded)
            {
                _logger.Log(LogLevel.Error, $"User {parsedClaim.UserName} cannot delete the user with id {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Message = "Cannot delete the user." });
            }

            _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} deleted a user with id {id}.");
            return NoContent();
        }

        // GET: api/Users
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            var parsedClaim = _myClaim.ParseAuthClaim(HttpContext);

            _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} is getting all users.");

            return _userManagerWrapper.GetUsers().Select(u => new User
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email
            });
        }

        // POST api/Users/authenticate/jwt
        [AllowAnonymous]
        [HttpPost("authenticate/jwt")]
        public async Task<IActionResult> AuthenticateAsJwtAsync([FromBody] UserCredential userCredential)
        {
            var user = await _userManagerWrapper.FindByNameAsync(userCredential.UserName);
            if (user == null)
            {
                _logger.Log(LogLevel.Debug, $"User {userCredential.UserName} cannot be authenticated because the user does not exist.");
                return Unauthorized();
            }

            if (!await _userManagerWrapper.CheckPasswordAsync(user, userCredential.Password))
            {
                _logger.Log(LogLevel.Debug, $"User {userCredential.UserName} cannot be authenticated because the password is not correct.");
                return Unauthorized();
            }

            var userRoles = await _userManagerWrapper.GetRolesAsync(user);

            var token = _jwtAuth.GetToken(userCredential.UserName, userRoles);
            _logger.Log(LogLevel.Information, $"User {userCredential.UserName} is authenticated.");
            return Ok(token);
        }

        // POST api/Users/authenticate/cookie
        [AllowAnonymous]
        [HttpPost("authenticate/cookie")]
        public async Task<IActionResult> AuthenticateAsCookieAsync([FromBody] UserCredential userCredential)
        {
            var user = await _userManagerWrapper.FindByNameAsync(userCredential.UserName);
            if (user == null)
            {
                _logger.Log(LogLevel.Debug, $"User {userCredential.UserName} cannot be authenticated because the user does not exist.");
                return Unauthorized();
            }

            if (!await _userManagerWrapper.CheckPasswordAsync(user, userCredential.Password))
            {
                _logger.Log(LogLevel.Debug, $"User {userCredential.UserName} cannot be authenticated because the password is not correct.");
                return Unauthorized();
            }

            var userRoles = await _userManagerWrapper.GetRolesAsync(user);

            await _cookieAuth.SignInAsync(userCredential.UserName, userRoles, HttpContext);
            _logger.Log(LogLevel.Information, $"User {userCredential.UserName} is authenticated.");
            return NoContent();
        }

        // POST api/Users/signout/cookie
        [HttpPost("signout/cookie")]
        public async Task<IActionResult> SignOutAsCookieAsync()
        {
            var parsedClaim = _myClaim.ParseAuthClaim(HttpContext);

            _logger.Log(LogLevel.Information, $"User {parsedClaim.UserName} is signing out.");
            await _cookieAuth.SignOutAsync(HttpContext);
            return NoContent();
        }
    }
}