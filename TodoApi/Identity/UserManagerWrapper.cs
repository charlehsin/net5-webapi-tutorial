using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TodoApi.Identity
{
    public class UserManagerWrapper : IUserManagerWrapper
    {
        private readonly UserManager<AppUser> _userManager;

        public UserManagerWrapper(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateAsync(AppUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<bool> CheckPasswordAsync(AppUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> IsLockedOutAsync(AppUser user)
        {
            return await _userManager.IsLockedOutAsync(user);
        }

        public async Task<IdentityResult> ResetAccessFailedCountAsync(AppUser user)
        {
            return await _userManager.ResetAccessFailedCountAsync(user);
        }

        public async Task<IdentityResult> AccessFailedAsync(AppUser user)
        {
            return await _userManager.AccessFailedAsync(user);
        }

        public async Task<IdentityResult> SetLockoutEndDateAsync(AppUser user, DateTimeOffset? lockoutEnd)
        {
            return await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
        }

        public async Task<IdentityResult> DeleteAsync(AppUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<AppUser> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<AppUser> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public IQueryable<AppUser> GetUsers()
        {
            return _userManager.Users;
        }

        public async Task<IdentityResult> AddToRoleAsync(AppUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(AppUser user, string role)
        {
            return await _userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task<IList<string>> GetRolesAsync(AppUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

    }
}