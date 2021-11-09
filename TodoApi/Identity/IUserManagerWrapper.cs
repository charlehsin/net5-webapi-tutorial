using System;
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
        /// Check if the user is locked out.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Ture if it is locked out.</returns>
        Task<bool> IsLockedOutAsync(AppUser user);

        /// <summary>
        /// Resets the access failed count for the target user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> ResetAccessFailedCountAsync(AppUser user);

        /// <summary>
        /// Increments the access failed count for the user as an asynchronous operation.
        /// If the failed access account is greater than or equal to the configured maximum number of attempts, the user will be locked out for the configured lockout time span.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> AccessFailedAsync(AppUser user);

        /// <summary>
        /// Locks out a user until the specified end date has passed. Setting a end date in the past immediately unlocks a user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="lockoutEnd"></param>
        /// <returns>IdentityResult</returns>
        Task<IdentityResult> SetLockoutEndDateAsync(AppUser user, DateTimeOffset? lockoutEnd);

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