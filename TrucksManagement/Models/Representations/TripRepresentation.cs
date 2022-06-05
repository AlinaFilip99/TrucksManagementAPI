using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrucksManagement.Models.Representations
{
    public class TripRepresentation
    {
        public TripRepresentation(Trip trip)
        {
            TripId = trip.TripId;
            UserId = trip.UserId;
            Country = trip.Country;
            Region = trip.Region;
            City = trip.City;
            Street = trip.Street;
            MoreAddress = trip.MoreAddress;
            Duration = trip.Duration;
            StartDateTime = trip.StartDateTime;
            EndDateTime = trip.EndDateTime;
            IsFinished = trip.IsFinished;
            IsDoneInTime = trip.IsDoneInTime;
            IsActive = trip.IsActive;
        }
        public TripRepresentation()
        {

        }
        public int TripId { get; set; }
        public string UserId { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string MoreAddress { get; set; }
        public string Duration { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public bool IsFinished { get; set; }
        public bool IsDoneInTime { get; set; }
        public bool IsActive { get; set; }

        internal Trip GetEntity(int id)
        {
            this.TripId = id;
            return new Trip
            {
                TripId = id,
                UserId = UserId,
                Country = Country,
                Region = Region,
                City = City,
                Street = Street,
                MoreAddress = MoreAddress,
                Duration = Duration,
                StartDateTime = StartDateTime,
                EndDateTime = EndDateTime,
                IsFinished = IsFinished,
                IsDoneInTime=IsDoneInTime,
                IsActive=IsActive,
            };
        }
        internal Trip GetEntityWithoutId(int id)
        {
            this.TripId = id;
            return new Trip
            {
                UserId = UserId,
                Country = Country,
                Region = Region,
                City = City,
                Street = Street,
                MoreAddress = MoreAddress,
                Duration = Duration,
                StartDateTime = StartDateTime,
                EndDateTime = EndDateTime,
                IsFinished=IsFinished,
                IsDoneInTime=IsDoneInTime,
                IsActive=IsActive,
            };
        }
    }
}
