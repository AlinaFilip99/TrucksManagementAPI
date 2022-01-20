using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrucksManagement.Models;
using TrucksManagement.Models.Representations;

namespace TrucksManagement.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<User> userManager, SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }
        [HttpPost]
        [Route("user/signup")]
        public async Task<IdentityResult> RegisterUserAsync(UserRepresentation user)
        {
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));//Manager
                await _roleManager.CreateAsync(new IdentityRole("User"));//Car
            }
            var newUser = user.GetEntityWithoutId(Guid.NewGuid().ToString());
            var result = await _userManager.CreateAsync(newUser, newUser.PasswordHash);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, user.UserRole);
                await _signInManager.SignInAsync(newUser, isPersistent: false);
            }

            return result;
        }
        [HttpPost]
        [Route("user/login")]
        public async Task<Microsoft.AspNetCore.Identity.SignInResult> SignInCredentials(UserRepresentation user)
        {
            User existentUser = await _userManager.FindByEmailAsync(user.Email);
            IList<string> userRoles = await _userManager.GetRolesAsync(existentUser);
            return await _signInManager.PasswordSignInAsync(existentUser.UserName, user.PasswordHash, false, false);
        }
        [HttpPost]
        [Route("user/roles")]
        public async Task<IList<string>> GetUserRoles(UserRepresentation user)
        {
            User existentUser = await _userManager.FindByEmailAsync(user.Email);
            return await _userManager.GetRolesAsync(existentUser);
        }
        [HttpPost]
        [Route("user/logout")]
        public async void SignOutUser()
        {
            await _signInManager.SignOutAsync();
        }
        [HttpGet]
        [Route("user/{email?}")]
        public async Task<UserRepresentation> GetByEmail(string email)
        {
            return new UserRepresentation(await _userManager.FindByEmailAsync(email));
        }
        [HttpGet]
        [Route("user/id/{userId?}")]
        public async Task<UserRepresentation> GetById(string userId)
        {
            return new UserRepresentation(await _userManager.FindByIdAsync(userId));
        }
        [HttpGet]
        [Route("user/trucks")]
        public async Task<IList<User>> GetTrucks()
        {
            return await _userManager.GetUsersInRoleAsync("User");
        }
        [HttpDelete]
        [Route("user/delete")]
        public async Task<IdentityResult> DeleteUser(string id)
        {
            User existentUser = await _userManager.FindByIdAsync(id);
            return await _userManager.DeleteAsync(existentUser);
        }
        [HttpPut]
        [Route("user/edit")]
        public async Task<IdentityResult> EditUser(UserRepresentation user)
        {
            User existentUser = await _userManager.FindByIdAsync(user.UserId);
            existentUser.UserName = user.UserName;
            existentUser.Email = user.Email;
            existentUser.PasswordHash = user.PasswordHash;
            existentUser.PlateNumber = user.PlateNumber;
            existentUser.PhoneNumber = user.PhoneNumber;
            return await _userManager.UpdateAsync(existentUser);
        }
    }
}
