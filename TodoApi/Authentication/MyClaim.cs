using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TodoApi.Authentication
{
    public class MyClaim : IMyClaim
    {
        public string ParseAuthClaim(HttpContext context)
        {
            var currentUser = context.User;    
            if (context == null || currentUser == null)
            {
                return string.Empty;
            }

            return ParseAuthClaim(currentUser);
        }

        public string ParseAuthClaim(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return string.Empty;
            }

            if (principal.HasClaim(c => c.Type == ClaimTypes.Name))    
            {    
                return principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            }

            return string.Empty;
        }
    }
}