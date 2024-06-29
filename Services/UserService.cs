using Inventory.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Inventory.Services
{
    public class UserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser?> GetById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IList<ApplicationUser>> GetAll()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<IdentityResult> Delete(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> Add(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> Update(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<ApplicationUser?> GetByPrincipal(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }

        public async Task<ApplicationUser?> GetByUserName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<IdentityResult> UpdatePassword(ApplicationUser user, string password)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<IList<string>> GetRoles(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> AddRoles(ApplicationUser user, IEnumerable<string> roles)
        {
            return await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task<IdentityResult> RemoveRoles(ApplicationUser user, IEnumerable<string> roles)
        {
            return await _userManager.RemoveFromRolesAsync(user, roles);
        }

        public async Task<IdentityResult> AddRole(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> RemoveRole(ApplicationUser user, string role)
        {
            return await _userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task<bool> HasRole(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> IsActive(ApplicationUser user)
        {
            return await HasRole(user, nameof(Roles.User));
        }

        public async Task<bool> IsAdmin(ApplicationUser user)
        {
            return await HasRole(user, nameof(Roles.Admin));
        }

        public async Task<IdentityResult> Activate(ApplicationUser user)
        {
            return await AddRole(user, nameof(Roles.User));
        }

        public async Task<IdentityResult> Deactivate(ApplicationUser user)
        {
            return await RemoveRole(user, nameof(Roles.User));
        }

    }
}
