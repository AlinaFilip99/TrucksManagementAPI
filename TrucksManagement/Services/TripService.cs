using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrucksManagement.Models;
using TrucksManagement.Repositories;

namespace TrucksManagement.Services
{
    public class TripService
    {
        protected readonly ITripRepository _tripRepository;

        public TripService(ITripRepository tripRepository)
        {
            this._tripRepository = tripRepository;
        }
        public IEnumerable<Trip> GetAll()
        {
            return _tripRepository.GetTrips();
        }
        public IEnumerable<Trip> GetTruckTrips(string truckId)
        {
            return _tripRepository.GetTripsByTruck(truckId);
        }
        public Trip GetById(int tripId)
        {
            return _tripRepository.GetTripById(tripId);
        }
        public int GetMaxId()
        {
            return _tripRepository.GetMaxId();
        }
        public void AddNewTrip(Trip trip)
        {
            _tripRepository.AddTrip(trip);
        }
        public void UpdateTrip(Trip trip)
        {
            _tripRepository.UpdateTrip(trip);
        }
        public void DeleteTrip(int id)
        {
            _tripRepository.DeleteTrip(id);
        }
        public void DeleteTruckTrips(string truckId)
        {
            _tripRepository.DeleteTripsByUser(truckId);
        }
    }
}
