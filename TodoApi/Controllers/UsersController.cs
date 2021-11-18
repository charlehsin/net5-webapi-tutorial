using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mime;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TodoApi.Authentication;
using TodoApi.Models;
using TodoApi.Identity;

namespace TodoApi.Controllers
{
    /// <summary>
    /// This is the examples controller to manage users and authentication.
    /// </summary>
    [Authorize(AuthenticationSchemes = GeneralAuth.AuthSchemes)]
    [Authorize(Roles = UserRole.RoleAdminOrUser)]
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

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <remarks>
        /// The password requires an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Passwords must be at least six characters long.
        ///
        /// Each user requires a unique email address.
        ///
        /// For tutorial and ease of testing purpose, this API does not require authentication.
        /// </remarks>
        /// <param name="userRegister">The target user UserRegister object.</param>
        /// <returns>A newly created user.</returns>
        /// <response code="201">Returns the newly created item.</response>
        /// <response code="409">If the user name already exists.</response>
        /// <response code="500">If the user creation fails.</response>
        [AllowAnonymous]
        [HttpPost]
        [ActionName("CreateAsync")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

            // TODO: This available role creation should be moved to other places. This should not be tied to user creation.
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
                    _logger.Log(LogLevel.Warning, $"Fails to add the user to role {userRegister.Role}.");
                }
            }

            return CreatedAtAction(nameof(CreateAsync), new User
            {
                Id = appUser.Id,
                UserName = appUser.UserName,
                Email = appUser.Email
            });
        }

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <remarks>
        /// For tutorial and ease of testing purpose, this API does not require authentication.
        /// </remarks>
        /// <param name="id">The user ID.</param>
        /// <returns></returns>
        /// <response code="204">If the user is deleted successfully.</response>
        /// <response code="404">If the user id does not exist.</response>
        /// <response code="500">If the user deletion fails.</response>
        [AllowAnonymous]
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get all the users.
        /// </summary>
        /// <remarks>
        /// This requires a user with Admin role or with User role.
        /// </remarks>
        /// <returns>The list of users.</returns>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="401">If this is not authorized.</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IEnumerable<User> GetUsers()
        {
            var parsedClaim = _myClaim.ParseAuthClaim(HttpContext);

            _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} with roles {parsedClaim.Roles} is getting all users.");

            return _userManagerWrapper.GetUsers().Select(u => new User
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email
            });
        }

        /// <summary>
        /// Unlock a user.
        /// </summary>
        /// <remarks>
        /// This requires a user with Admin role.
        /// </remarks>
        /// <param name="id">The target user ID.</param>
        /// <returns></returns>
        /// <response code="204">If the user is unlocked successfully.</response>
        /// <response code="403">If the user role is allowed.</response>
        /// <response code="404">If the user id does not exist.</response>
        /// <response code="500">If the user unlocking fails.</response>
        [Authorize(Roles = UserRole.RoleAdmin)]
        [HttpPost("{id}/unlock")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UnlockUserAsync(string id)
        {
            var parsedClaim = _myClaim.ParseAuthClaim(HttpContext);

            var user = await _userManagerWrapper.FindByIdAsync(id);
            if (user == null)
            {
                _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} tries to unlock a user with id {id} that does not exist.");
                return NotFound();
            }

            var result = await _userManagerWrapper.ResetAccessFailedCountAsync(user);
            if (!result.Succeeded)
            {
                _logger.Log(LogLevel.Error, $"User {parsedClaim.UserName} cannot reset the access failed count for the user with id {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Message = "Cannot unlock the user." });
            }

            result = await _userManagerWrapper.SetLockoutEndDateAsync(user, null);
            if (!result.Succeeded)
            {
                _logger.Log(LogLevel.Error, $"User {parsedClaim.UserName} cannot set the lockout end date for the user with id {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Message = "Cannot unlock the user." });
            }

            _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} unlocked a user with id {id}.");
            return NoContent();
        }

        /// <summary>
        /// Authenticate the user and get the JWT.
        /// </summary>
        /// <param name="userCredential">The UserCredential object.</param>
        /// <returns>JWT.</returns>
        /// <response code="200">Returns the JWT.</response>
        /// <response code="401">If the user authentication fails.</response>
        [AllowAnonymous]
        [HttpPost("authenticate/jwt")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(String))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AuthenticateAsJwtAsync([FromBody] UserCredential userCredential)
        {
            var user = await TryAuthenticateAsync(userCredential).ConfigureAwait(false);
            if (user == null)
            {
                return Unauthorized();
            }

            var userRoles = await _userManagerWrapper.GetRolesAsync(user);

            var token = _jwtAuth.GetToken(userCredential.UserName, userRoles);
            _logger.Log(LogLevel.Information, $"User {userCredential.UserName} is authenticated.");
            return Ok(token);
        }

        /// <summary>
        /// Authenticate the user and create the auth cookie.
        /// </summary>
        /// <param name="userCredential">The UserCredential object.</param>
        /// <returns></returns>
        /// <response code="204">If the user is authenticated and the auth cookie is created.</response>
        /// <response code="401">If the user authentication fails.</response>
        [AllowAnonymous]
        [HttpPost("authenticate/cookie")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AuthenticateAsCookieAsync([FromBody] UserCredential userCredential)
        {
            var user = await TryAuthenticateAsync(userCredential).ConfigureAwait(false);
            if (user == null)
            {
                return Unauthorized();
            }

            var userRoles = await _userManagerWrapper.GetRolesAsync(user);

            await _cookieAuth.SignInAsync(userCredential.UserName, userRoles, HttpContext);
            _logger.Log(LogLevel.Information, $"User {userCredential.UserName} is authenticated.");
            return NoContent();
        }

        /// <summary>
        /// Sign in Facebook and then get the JWT.
        /// </summary>
        /// <remarks>
        /// The authentication flow is the following.
        ///
        /// 1. The front-end application sends this "GET" API request.
        ///
        /// 2. Upon receiving the request, this method goes to the "challenge" flow. ASP.NET Core's internal FacebookHandler does the real "challenge" flow
        /// to prepare for the redirection information. The redirection information includes the target provider's AuthorizationEndpoint path, the client-id, and the redirection_uri, etc.
        /// ASP.NET Core internally uses "/signin-facebook" as the redirection_uri. We do not need to change this. We only need to configure this path at the external
        /// OAuth 2.0 provider, e.g., Facebook, side. Then Redirection (302) is returned to the front-end application.
        ///
        /// 3. The front-end application gets Redirection (302) and redirects to the target Facebook URL to perform the authentication at Facebook.
        ///
        /// 4. When Facebook finishes the authentication, it responds with another Redirection (302) to the front-end application. This redirection location is XXXX/signin-facebook.
        ///
        /// 5. The front-end application gets Redirection (302) and redirects to our ASP.NET Core Web API. ASP.NET Web API internally will process this XXXX/signin-facebook request,
        /// by working with the external OAuth 2.0 provider, e.g., Facebook.
        ///
        /// 6. After the above is done, ASP.NET Core internally responds with yet another Redirection (302) to the front-end. This time, the redirection location is this "GET" API path.
        ///
        /// 7. Then front-end application gets Redirection (302) and redirects to this "GET" API.
        ///
        /// 8. Upon receiving the request, this method goes to the "authenticate" flow. And eventually returns with a JWT.
        /// </remarks>
        /// <returns>JWT.</returns>
        /// <response code="200">Returns the JWT.</response>
        /// <response code="302">If the challenge works, and the authentication redirection location is obtained.</response>
        /// <response code="401">If the challenge fails because the authentication redirection location cannot be obtained.</response>
        [AllowAnonymous]
        [HttpGet("authenticate/facebook")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignInFacebookAsync()
        {
            var authScheme = FacebookDefaults.AuthenticationScheme;

            // Try to authenticate.
            var authResult = await Request.HttpContext.AuthenticateAsync(authScheme);
            if (!authResult.Succeeded
                || authResult?.Principal == null
                || !authResult.Principal.Identities.Any(id => id.IsAuthenticated)
                || string.IsNullOrEmpty(authResult.Properties.GetTokenValue("access_token")))
            {
                _logger.Log(LogLevel.Debug, $"Not authenticated via {authScheme} yet. Challenging now....");

                // Challenge the Facebook OAuth 2.0 provider.
                await Request.HttpContext.ChallengeAsync(authScheme, new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                    IssuedUtc = DateTimeOffset.UtcNow,
                    // We provide this API's own path here so that the final redirection can go to this method.
                    RedirectUri = Url.Action("SignInFacebookAsync")
                });

                // Get the location response header.
                if (Response.Headers.TryGetValue("location", out var locationResponseHeader))
                {
                    _logger.Log(LogLevel.Debug, $"Not authenticated via {authScheme} yet. The redirection location is {locationResponseHeader}.");
                    return Redirect(locationResponseHeader);
                }

                _logger.Log(LogLevel.Debug, $"Challenge via {authScheme} failed.");
                return Unauthorized();
            }

            var claimsIdentity = authResult.Principal.Identities.First(id => id.IsAuthenticated);
            var email = claimsIdentity.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            _logger.Log(LogLevel.Debug, $"User {claimsIdentity.Name} with email {email ?? string.Empty} is authenticated via {authScheme}.");

            // Then you need to get the role and user ID based on the user name.
            // In real world scenario, you may have other flows to add the target user from external OAuth 2.0 provider into ASP Identity.
            // This is beyond the scope of this sample codes.
            // For tutorial purpose here, we will use fixed Admin role.

            // In the API design of this sample codes, the APIs will only accept JWT Bearer authentication or Cookies authentication.
            // Therefore, we need to get JWT or create the auth cookie.
            // For tutorial purpose, we use JWT.

            // In real world scenario, you may want to use auth.Properties.ExpiresUtc to set your JWT expiration or auth cookie expiration accordingly.

            var token = _jwtAuth.GetToken(claimsIdentity.Name, new List<string> { UserRole.RoleAdmin });
            _logger.Log(LogLevel.Information, $"User {claimsIdentity.Name} has the JWT now.");
            return Ok(token);
        }

        /// <summary>
        /// Sign out and delete the auth cookie.
        /// </summary>
        /// <remarks>
        /// This requires a user with Admin role or with User role.
        /// </remarks>
        /// <returns></returns>
        /// <response code="204">If the user is signed out and the auth cookie is deleted.</response>
        /// <response code="401">If this is not authorized.</response>
        [HttpPost("signout/cookie")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignOutAsCookieAsync()
        {
            var parsedClaim = _myClaim.ParseAuthClaim(HttpContext);

            _logger.Log(LogLevel.Information, $"User {parsedClaim.UserName} is signing out.");
            await _cookieAuth.SignOutAsync(HttpContext);
            return NoContent();
        }

        /// <summary>
        /// Try to authenticate with the target credential.
        /// We also check if the user is locked out or not.
        /// </summary>
        /// <param name="userCredential">The UserCredential object.</param>
        /// <returns>The AppUser.</returns>
        private async Task<AppUser> TryAuthenticateAsync(UserCredential userCredential)
        {
            var user = await _userManagerWrapper.FindByNameAsync(userCredential.UserName);
            if (user == null)
            {
                _logger.Log(LogLevel.Debug, $"User {userCredential.UserName} cannot be authenticated because the user does not exist.");
                return null;
            }

            var isLockedOut = await _userManagerWrapper.IsLockedOutAsync(user);
            if (isLockedOut)
            {
                _logger.Log(LogLevel.Debug, $"User {userCredential.UserName} cannot be authenticated because the user is locked out.");
                return null;
            }

            IdentityResult result;
            if (!await _userManagerWrapper.CheckPasswordAsync(user, userCredential.Password))
            {
                _logger.Log(LogLevel.Debug, $"User {userCredential.UserName} cannot be authenticated because the password is not correct.");

                result = await _userManagerWrapper.AccessFailedAsync(user);
                if (!result.Succeeded)
                {
                    _logger.Log(LogLevel.Error, $"Cannot increment the access failed account for User {userCredential.UserName}.");
                }

                isLockedOut = await _userManagerWrapper.IsLockedOutAsync(user);
                if (isLockedOut)
                {
                    _logger.Log(LogLevel.Debug, $"User {userCredential.UserName} is locked out.");
                }

                return null;
            }

            result = await _userManagerWrapper.ResetAccessFailedCountAsync(user);
            if (!result.Succeeded)
            {
                _logger.Log(LogLevel.Error, $"Cannot reset the access failed account for User {userCredential.UserName}.");
            }

            return user;
        }
    }
}