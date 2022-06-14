using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrucksManagement.Models;
using TrucksManagement.Models.Representations;
using TrucksManagement.Repositories;

namespace TrucksManagement.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        protected readonly ITripRepository _tripRepository;
        public UserController(UserManager<User> userManager, SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager, ITripRepository tripRepository)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._tripRepository = tripRepository;
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
        public async Task<IList<UserRepresentation>> GetTrucks()
        {
            var trucks = await _userManager.GetUsersInRoleAsync("User");
            return trucks.Select(s => new UserRepresentation(s)).ToList();
        }
        [HttpGet]
        [Route("user/trucks/{adminId?}")]
        public async Task<IList<UserRepresentation>> GetTrucksByAdmin(string adminId)
        {
            var trucks=await _userManager.GetUsersInRoleAsync("User");
            return trucks.Where((x) => x.AdminID == adminId).Select(s => new UserRepresentation(s)).ToList();
        }
        [HttpGet]
        [Route("user/available")]
        public async Task<IList<UserRepresentation>> GetTrucksByAvailability(string adminId, string startDate, string endDate)
        {
            var testStartDate = DateTime.Parse(startDate);
            var testEndDate = DateTime.Parse(endDate);
            var trucks = await _userManager.GetUsersInRoleAsync("User");
            var adminTrucks= trucks.Where((x) => x.AdminID == adminId).Select(s => new UserRepresentation(s)).ToList();
            List<UserRepresentation> availableTrucks=new List<UserRepresentation>();
            foreach(var truck in adminTrucks)
            {
                var truckTrips = _tripRepository.GetTripsByTruck(truck.UserId).OrderBy(x => x.StartDateTime).ToList();
                bool isValid = true;
                foreach(var trip in truckTrips)
                {
                    if((testStartDate >=DateTime.Parse(trip.StartDateTime) && testStartDate <= DateTime.Parse(trip.EndDateTime)) ||
                       (testEndDate >= DateTime.Parse(trip.StartDateTime) && testEndDate <= DateTime.Parse(trip.EndDateTime)))
                    {
                        isValid = false;
                    }
                }
                if (isValid)
                {
                    availableTrucks.Add(truck);
                }
            }
            return availableTrucks;
        }
        [HttpDelete]
        [Route("user/delete")]
        public async Task<IdentityResult> DeleteUser(string id)
        {
            User existentUser = await _userManager.FindByIdAsync(id);
            if (existentUser!=null)
            {
                var role = await _userManager.GetRolesAsync(existentUser);
                if (role.Count > 0)
                {
                    if (role[0] == "Admin")
                    {
                        var trucks = await _userManager.GetUsersInRoleAsync("User");
                        var adminTrucks = trucks.Where((x) => x.AdminID == existentUser.Id).ToList();
                        foreach(var truck in adminTrucks)
                        {
                            await _userManager.DeleteAsync(truck);
                        }
                    }
                    else{
                        _tripRepository.DeleteTripsByUser(existentUser.Id);
                    }
                }
            }
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
