using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrucksManagement.Models
{
    public class User : IdentityUser
    {
        public ICollection<Trip> Trips { get; set; }
        public string PlateNumber { get; set; }
        public int DriversNumber { get; set; }
        public string AdminID { get; set; }
    }
}
