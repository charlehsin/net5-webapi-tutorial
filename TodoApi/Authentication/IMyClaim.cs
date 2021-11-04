using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TodoApi.Authentication
{
    public interface IMyClaim
    {
        /// <summary>
        /// Parse the authentication claim.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>ParsedClaim</returns>
        ParsedClaim ParseAuthClaim(HttpContext context);

        /// <summary>
        /// Parse the authentication claim.
        /// </summary>
        /// <param name="principal"></param>
        /// <returns>ParsedClaim</returns>
        ParsedClaim ParseAuthClaim(ClaimsPrincipal principal);
    }
}