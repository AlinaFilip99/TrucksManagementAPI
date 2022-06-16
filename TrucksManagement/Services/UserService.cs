using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrucksManagement.Models;

namespace TrucksManagement.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            UserManager<User> userManager, SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }
        public async Task<IdentityResult> UpdateAccount(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<SignInResult> SignInCredentials(User user, string passwordHash)
        {
            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            return await _signInManager.PasswordSignInAsync(user.UserName, passwordHash, false, false);
        }

        public async void SignOut()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<IList<string>> GetUserRoles(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }
        public async Task<IdentityResult> RegisterUser(User user, string userRole)
        {
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));//Manager
                await _roleManager.CreateAsync(new IdentityRole("User"));//Car
            }
            var result = await _userManager.CreateAsync(user, user.PasswordHash);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, userRole);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return result;
        }
        public async Task<User> GetAccountByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<User> GetAccountById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IList<User>> GetTrucks()
        {
            return await _userManager.GetUsersInRoleAsync("User");
        }

        public async Task<IdentityResult> DeleteUser(User user)
        {
            return await _userManager.DeleteAsync(user);
        }
    }
}
