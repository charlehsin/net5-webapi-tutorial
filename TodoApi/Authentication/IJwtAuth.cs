using System.Collections.Generic;

namespace TodoApi.Authentication
{
    public interface IJwtAuth
    {
        /// <summary>
        /// Get the JWT.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roles"></param>
        /// <returns>JWT</returns>
        string GetToken(string userName, IList<string> roles);
    }
}