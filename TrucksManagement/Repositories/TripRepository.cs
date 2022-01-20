using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrucksManagement.Models;

namespace TrucksManagement.Repositories
{
    public class TripRepository : ITripRepository
    {
        protected readonly ApplicationContext dbContext;
        public TripRepository(ApplicationContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Trip AddTrip(Trip trip)
        {
            var result = dbContext.Add(trip);
            dbContext.SaveChanges();
            return result.Entity;
        }

        public void DeleteTrip(int tripId)
        {
            var result = dbContext.Trips
                .FirstOrDefault(e => e.TripId == tripId);
            if (result != null)
            {
                dbContext.Remove(result);
                dbContext.SaveChanges();
            }
        }

        public int GetMaxId()
        {
            return dbContext.Trips.Select(s => s.TripId).Max();
        }

        public Trip GetTripById(int tripId)
        {
            return dbContext.Trips
                .FirstOrDefault(e => e.TripId == tripId);
        }

        public IEnumerable<Trip> GetTrips()
        {
            return dbContext.Trips.ToList();
        }

        public IEnumerable<Trip> GetTripsByTruck(string truckId)
        {
            return dbContext.Trips.Where(s => s.UserId == truckId).ToList();
        }

        public Trip UpdateTrip(Trip trip)
        {
            var result = dbContext.Trips.FirstOrDefault(e => e.TripId == trip.TripId);

            if (result != null)
            {
                result.TripId= trip.TripId;
                result.UserId = trip.UserId;
                result.Country = trip.Country;
                result.Region = trip.Region;
                result.City = trip.City;
                result.Street = trip.Street;
                result.MoreAddress = trip.MoreAddress;
                result.Duration = trip.Duration;
                result.StartDateTime = trip.StartDateTime;
                result.EndDateTime = trip.EndDateTime;

                dbContext.SaveChanges();

                return result;
            }

            return null;
        }
    }
}
