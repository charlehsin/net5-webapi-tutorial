using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TodoApi.Authentication
{
    public class ParsedClaim
    {
        public string UserName { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class MyClaim : IMyClaim
    {
        public ParsedClaim ParseAuthClaim(HttpContext context)
        {
            var currentUser = context.User;
            if (context == null || currentUser == null)
            {
                return new ParsedClaim
                {
                    UserName = string.Empty,
                    Roles = new List<string>()
                };
            }

            return ParseAuthClaim(currentUser);
        }

        public ParsedClaim ParseAuthClaim(ClaimsPrincipal principal)
        {
            var parsedClaim = new ParsedClaim
            {
                UserName = string.Empty,
                Roles = new List<string>()
            };

            if (principal == null)
            {
                return parsedClaim;
            }

            if (principal.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                parsedClaim.UserName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            }

            if (principal.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                var targetList = principal.Claims.Where(c => c.Type == ClaimTypes.Role);
                foreach (var claim in targetList)
                {
                    parsedClaim.Roles.Add(claim.Value);
                }
            }

            return parsedClaim;
        }
    }
}