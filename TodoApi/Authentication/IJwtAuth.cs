using System.Collections.Generic;

namespace TodoApi.Authentication
{
    public interface IJwtAuth
    {
        /// <summary>
        /// Get the JWT.
        /// </summary>
        /// <param name="userName">The target user name.</param>
        /// <param name="roles">The list of roles this user has.</param>
        /// <returns>JWT.</returns>
        string GetToken(string userName, IList<string> roles);
    }
}