using Microsoft.AspNetCore.Http;

namespace TodoApi.Authentication
{
    public interface IMyClaim
    {
        /// <summary>
        /// Parse the authentication claim.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>user name</returns>
        string ParseAuthClaim(HttpContext context);
    }
}