using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TodoApi.Authentication
{
    public class GeneralAuth
    {
        public const string AuthSchemes =
            CookieAuthenticationDefaults.AuthenticationScheme + "," +
            JwtBearerDefaults.AuthenticationScheme;

        /// <summary>
        /// Get the authentication claims.
        /// </summary>
        /// <param name="userName">The target user name.</param>
        /// <param name="roles">The list of roles this user has.</param>
        /// <returns>The list of claims.</returns>
        public List<Claim> GetAuthClaims(string userName, IList<string> roles)
        {
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName)
                };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            return authClaims;
        }
    }
}