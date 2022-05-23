using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrucksManagement.Models;

namespace TrucksManagement.Repositories
{
    public interface ITripRepository
    {
        IEnumerable<Trip> GetTrips();
        IEnumerable<Trip> GetTripsByTruck(string truckId);
        Trip AddTrip(Trip trip);
        Trip UpdateTrip(Trip trip);
        void DeleteTrip(int tripId);
        void DeleteTripsByUser(string truckId);
        int GetMaxId();
        Trip GetTripById(int tripId);
    }
}
