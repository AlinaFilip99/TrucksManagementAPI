using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrucksManagement.Models;
using TrucksManagement.Models.Representations;
using TrucksManagement.Services;

namespace TrucksManagement.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        protected readonly TripService _tripService;
        public UserController(UserService userService, TripService tripService)
        {
            this._tripService = tripService;
            this._userService = userService;
        }
        [HttpPost]
        [Route("user/signup")]
        public async Task<IdentityResult> RegisterUserAsync(UserRepresentation user) 
        {
            var newUser = user.GetEntityWithoutId(Guid.NewGuid().ToString());
            return await _userService.RegisterUser(newUser, user.UserRole);
        }
        [HttpPost]
        [Route("user/login")]
        public async Task<Microsoft.AspNetCore.Identity.SignInResult> SignInCredentials(UserRepresentation user)
        {
            User existentUser = await _userService.GetAccountByEmail(user.Email);
            return await _userService.SignInCredentials(existentUser, user.PasswordHash);
        }
        [HttpPost]
        [Route("user/roles")]
        public async Task<IList<string>> GetUserRoles(UserRepresentation user)
        {
            User existentUser = await _userService.GetAccountByEmail(user.Email);
            return await _userService.GetUserRoles(existentUser);
        }
        [HttpPost]
        [Route("user/logout")]
        public void SignOutUser()
        {
            _userService.SignOut();
        }
        [HttpGet]
        [Route("user/{email?}")]
        public async Task<UserRepresentation> GetByEmail(string email)
        {
            return new UserRepresentation(await _userService.GetAccountByEmail(email));
        }
        [HttpGet]
        [Route("user/id/{userId?}")]
        public async Task<UserRepresentation> GetById(string userId)
        {
            return new UserRepresentation(await _userService.GetAccountById(userId));
        }
        [HttpGet]
        [Route("user/trucks")]
        public async Task<IList<UserRepresentation>> GetTrucks()
        {
            var trucks = await _userService.GetTrucks();
            return trucks.Select(s => new UserRepresentation(s)).ToList();
        }
        [HttpGet]
        [Route("user/trucks/{adminId?}")]
        public async Task<IList<UserRepresentation>> GetTrucksByAdmin(string adminId)
        {
            var trucks=await _userService.GetTrucks();
            return trucks.Where((x) => x.AdminID == adminId).Select(s => new UserRepresentation(s)).ToList();
        }
        [HttpGet]
        [Route("user/available")]
        public async Task<IList<UserRepresentation>> GetTrucksByAvailability(string adminId, string startDate, string endDate)
        {
            var testStartDate = DateTime.Parse(startDate);
            var testEndDate = DateTime.Parse(endDate);
            var trucks = await _userService.GetTrucks();
            var adminTrucks= trucks.Where((x) => x.AdminID == adminId).Select(s => new UserRepresentation(s)).ToList();
            List<UserRepresentation> availableTrucks=new List<UserRepresentation>();
            foreach(var truck in adminTrucks)
            {
                var truckTrips = _tripService.GetTruckTrips(truck.UserId).OrderBy(x => x.StartDateTime).ToList();
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
            User existentUser = await _userService.GetAccountById(id);
            if (existentUser!=null)
            {
                var role = await _userService.GetUserRoles(existentUser);
                if (role.Count > 0)
                {
                    if (role[0] == "Admin")
                    {
                        var trucks = await _userService.GetTrucks();
                        var adminTrucks = trucks.Where((x) => x.AdminID == existentUser.Id).ToList();
                        foreach(var truck in adminTrucks)
                        {
                            await _userService.DeleteUser(truck);
                        }
                    }
                    else{
                        _tripService.DeleteTruckTrips(existentUser.Id);
                    }
                }
            }
            return await _userService.DeleteUser(existentUser);
        }
        [HttpPut]
        [Route("user/edit")]
        public async Task<IdentityResult> EditUser(UserRepresentation user)
        {
            User existentUser = await _userService.GetAccountById(user.UserId);
            existentUser.UserName = user.UserName;
            existentUser.Email = user.Email;
            existentUser.PasswordHash = user.PasswordHash;
            existentUser.PlateNumber = user.PlateNumber;
            existentUser.PhoneNumber = user.PhoneNumber;
            return await _userService.UpdateAccount(existentUser);
        }
    }
}
