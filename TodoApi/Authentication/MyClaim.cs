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
            if (currentUser == null)
            {
                return string.Empty;
            }

            if (currentUser.HasClaim(c => c.Type == ClaimTypes.Name))    
            {    
                return currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            }

            return string.Empty;
        }
    }
}