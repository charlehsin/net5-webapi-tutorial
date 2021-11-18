using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TodoApi.Authentication
{
    public interface IMyClaim
    {
        /// <summary>
        /// Parse the authentication claim.
        /// </summary>
        /// <param name="context">The HttpContext.</param>
        /// <returns>The ParsedClaim object.</returns>
        ParsedClaim ParseAuthClaim(HttpContext context);

        /// <summary>
        /// Parse the authentication claim.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal.</param>
        /// <returns>The ParsedClaim object.</returns>
        ParsedClaim ParseAuthClaim(ClaimsPrincipal principal);
    }
}