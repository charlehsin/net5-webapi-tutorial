using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TodoApi.Authentication
{
    public interface ICookieAuth
    {
        /// <summary>
        /// Authenticate the user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<bool> AuthenticateAsync(string username, string password,
            HttpContext context);

        /// <summary>
        /// Sign out and delete the current auth cookie.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task SignOutAsync(HttpContext context);
    }
}