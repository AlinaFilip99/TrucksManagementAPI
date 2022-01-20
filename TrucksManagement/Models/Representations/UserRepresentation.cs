using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrucksManagement.Models.Representations
{
    public class UserRepresentation
    {
        public UserRepresentation(User user)
        {
            UserId = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            PasswordHash = user.PasswordHash;
            PlateNumber = user.PlateNumber;
            DriversNumber = user.DriversNumber;
            PhoneNumber = user.PhoneNumber;
        }
        public UserRepresentation()
        {

        }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string UserRole { get; set; }
        public string PlateNumber { get; set; }
        public int DriversNumber { get; set; }
        public string PhoneNumber { get; set; }

        internal User GetEntity(string id)
        {
            this.UserId = id;
            return new User
            {
                Id = id,
                UserName = UserName,
                Email = Email,
                PasswordHash = PasswordHash,
                PlateNumber=PlateNumber,
                DriversNumber=DriversNumber,
                PhoneNumber=PhoneNumber
            };
        }
        internal User GetEntityWithoutId(string id)
        {
            this.UserId = id;
            return new User
            {
                UserName = UserName,
                Email = Email,
                PasswordHash = PasswordHash,
                PlateNumber = PlateNumber,
                DriversNumber = DriversNumber,
                PhoneNumber = PhoneNumber
            };
        }
    }
}
