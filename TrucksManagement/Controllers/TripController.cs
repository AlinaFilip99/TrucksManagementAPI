using Microsoft.AspNetCore.Http;
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
    public class TripController : ControllerBase
    {
        private readonly TripService _tripService;
        private readonly UserService _userService;

        public TripController(TripService tripService, UserService userService)
        {
            this._tripService = tripService;
            this._userService = userService;
        }

        [HttpGet]
        [Route("trip")]
        public IEnumerable<TripRepresentation> Get()
        {
            return _tripService.GetAll().Select(s => new TripRepresentation(s));
        }

        [HttpGet]
        [Route("trip/user/{adminId?}")]
        public async Task<IEnumerable<TripRepresentation>> GetAllByAdmin(string adminId)
        {
            var trucks = await _userService.GetTrucks();
            trucks=trucks.Where((x) => x.AdminID == adminId).ToList();
            List<Trip> trips= new List<Trip>();
            if (trucks.Count!=0)
            {
                foreach (var truck in trucks)
                {
                    var anterior = trips;
                    trips = anterior.Concat(_tripService.GetTruckTrips(truck.Id)).ToList();
                }
            }
            return trips.Select(s => new TripRepresentation(s));
        }

        [HttpGet]
        [Route("trip/{tripId?}")]
        public TripRepresentation GetById(int tripId)
        {
            var trip = new TripRepresentation(_tripService.GetById(tripId));
            return trip;
        }

        [HttpGet]
        [Route("trip/truck/{truckId?}")]
        public IEnumerable<TripRepresentation> GetByTruckId(string truckId)
        {
            return _tripService.GetTruckTrips(truckId).Select(s => new TripRepresentation(s));
        }

        [HttpPost]
        [Route("trip")]
        public TripRepresentation Post(TripRepresentation trip)
        {
            var max = 1;
            try
            {
                max = _tripService.GetMaxId();
            }
            catch
            {
                max = 0;
            }

            var newTrip = trip.GetEntityWithoutId(max + 1);
            _tripService.AddNewTrip(newTrip);
            return trip;
        }

        [HttpPut]
        [Route("trip")]
        public TripRepresentation Put(TripRepresentation trip)
        {
            Trip newTrip;
            newTrip = trip.GetEntity(trip.TripId);
            _tripService.UpdateTrip(newTrip);
            return trip;
        }

        [HttpDelete]
        [Route("trip")]
        public int Delete(int id)
        {
            _tripService.DeleteTrip(id);
            return id;
        }
    }
}
