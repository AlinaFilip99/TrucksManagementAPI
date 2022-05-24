using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrucksManagement.Models
{
    public class Trip
    {
        public int TripId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string MoreAddress { get; set; }
        public string Duration { get; set; } // 16:00
        public string StartDateTime { get; set; } // 2/16/2008 12:15:12 PM
        public string EndDateTime { get; set; } // 2/16/2008 12:15:12 PM
        public bool IsFinished { get; set; }
        public bool IsDoneInTime { get; set; }
    }
}
