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
        /// <param name="userName">The target user name.</param>
        /// <param name="roles">The list of roles this user has.</param>
        /// <param name="context">The HttpContext.</param>
        /// <returns>A Task representing the completed operation.</returns>
        Task SignInAsync(string userName, IList<string> roles,
            HttpContext context);

        /// <summary>
        /// Sign out and delete the current auth cookie.
        /// </summary>
        /// <param name="context">The HttpContext.</param>
        /// <returns>A Task representing the completed operation.</returns>
        Task SignOutAsync(HttpContext context);
    }
}