using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TodoApi.Authentication
{
    public interface ICookieAuth
    {
        /// <summary>
        /// Sign in and create the auth cookie.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roles"></param>
        /// <param name="context"></param>
        Task SignInAsync(string userName, IList<string> roles,
            HttpContext context);

        /// <summary>
        /// Sign out and delete the current auth cookie.
        /// </summary>
        /// <param name="context"></param>
        Task SignOutAsync(HttpContext context);
    }
}