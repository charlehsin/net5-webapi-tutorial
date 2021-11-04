using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TodoApi.Identity
{
    public interface IRoleManagerWrapper
    {
        /// <summary>
        /// Create the target role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> CreateAsync(string role);

        /// <summary>
        /// Check if the role exists already.
        /// </summary>
        /// <param name="role"></param>
        /// <returns>True if it exists</returns>
        Task<bool> RoleExistsAsync(string role);
    }
}