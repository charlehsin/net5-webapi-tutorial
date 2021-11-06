using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TodoApi.Identity
{
    public interface IUserManagerWrapper
    {
        /// <summary>
        /// Create anew user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> CreateAsync(AppUser user, string password);

        /// <summary>
        /// Check if the password is valid for the target user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns>True if it is valid.</returns>
        Task<bool> CheckPasswordAsync(AppUser user, string password);

        /// <summary>
        /// Delete the target user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IdentityResult> DeleteAsync(AppUser user);

        /// <summary>
        /// Find the target user by name.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>AppUser</returns>
        Task<AppUser> FindByNameAsync(string userName);

        /// <summary>
        /// Find the target user by ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>AppUser</returns>
        Task<AppUser> FindByIdAsync(string userId);

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns>users</returns>
        IQueryable<AppUser> GetUsers();

        /// <summary>
        /// Add the role to the user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> AddToRoleAsync(AppUser user, string role);

        /// <summary>
        /// Remove the role from the user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<IdentityResult> RemoveFromRoleAsync(AppUser user, string role);

        /// <summary>
        /// Get the roles of the target user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>list of roles</returns>
        Task<IList<string>> GetRolesAsync(AppUser user);
    }
}