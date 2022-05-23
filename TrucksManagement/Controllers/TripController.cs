using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrucksManagement.Models;
using TrucksManagement.Models.Representations;
using TrucksManagement.Repositories;

//https://docs.microsoft.com/en-us/dotnet/api/system.timespan.parse?view=net-6.0#System_TimeSpan_Parse_System_String_
//https://docs.microsoft.com/en-us/dotnet/api/system.datetime.parse?view=net-6.0#System_DateTime_Parse_System_String_

namespace TrucksManagement.Controllers
{
    [ApiController]
    public class TripController : ControllerBase
    {
        protected readonly ITripRepository _tripRepository;
        private readonly UserManager<User> _userManager;

        public TripController(ITripRepository tripRepository, UserManager<User> userManager)
        {
            this._tripRepository = tripRepository;
            this._userManager = userManager;
        }

        [HttpGet]
        [Route("trip")]
        public IEnumerable<TripRepresentation> Get()
        {
            return _tripRepository.GetTrips().Select(s => new TripRepresentation(s));
        }

        [HttpGet]
        [Route("trip/user/{adminId?}")]
        public async Task<IEnumerable<TripRepresentation>> GetAllByAdmin(string adminId)
        {
            var trucks = await _userManager.GetUsersInRoleAsync("User");
            trucks=trucks.Where((x) => x.AdminID == adminId).ToList();
            List<Trip> trips= new List<Trip>();
            if (trucks.Count!=0)
            {
                foreach (var truck in trucks)
                {
                    var anterior = trips;
                    trips = anterior.Concat(_tripRepository.GetTripsByTruck(truck.Id)).ToList();
                }
            }
            return trips.Select(s => new TripRepresentation(s));
        }

        [HttpGet]
        [Route("trip/{tripId?}")]
        public TripRepresentation GetById(int tripId)
        {
            var trip = new TripRepresentation(_tripRepository.GetTripById(tripId));
            return trip;
        }

        [HttpGet]
        [Route("trip/truck/{truckId?}")]
        public IEnumerable<TripRepresentation> GetByTruckId(string truckId)
        {
            return _tripRepository.GetTripsByTruck(truckId).Select(s => new TripRepresentation(s));
        }

        [HttpPost]
        [Route("trip")]
        public TripRepresentation Post(TripRepresentation trip)
        {
            var max = 1;
            try
            {
                max = _tripRepository.GetMaxId();
            }
            catch
            {
                max = 0;
            }

            var newTrip = trip.GetEntityWithoutId(max + 1);
            _tripRepository.AddTrip(newTrip);
            return trip;
        }

        [HttpPut]
        [Route("trip")]
        public TripRepresentation Put(TripRepresentation trip)
        {
            Trip newTrip;
            newTrip = trip.GetEntity(trip.TripId);
            _tripRepository.UpdateTrip(newTrip);
            return trip;
        }

        [HttpDelete]
        [Route("trip")]
        public int Delete(int id)
        {
            _tripRepository.DeleteTrip(id);
            return id;
        }
    }
}
