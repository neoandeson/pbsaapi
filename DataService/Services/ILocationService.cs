using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using DataService.Components.Location;
using DataService.Constants;
using DataService.Models;
using DataService.Repositories;
using DataService.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DataService.Services
{
    public interface ILocationService
    {
        void AddCustomerLocation(string customerId, int bookingId, double longitude, double latitude);
        
        List<CustomerLocations> GetCustomerLocations(string customerId, int bookingId);
        
        double CheckCustomerLocations(string customerId, int bookingId);

        bool InitCityDistrictDatabase();
    }

    public class LocationService : ILocationService
    {
        private readonly ICustomerLocationRepository _locationRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IDistrictRepository _districtRepository;

        public LocationService(ICustomerLocationRepository locationRepository, IBookingRepository bookingRepository, ICityRepository cityRepository, IDistrictRepository districtRepository)
        {
            _locationRepository = locationRepository;
            _bookingRepository = bookingRepository;
            _cityRepository = cityRepository;
            _districtRepository = districtRepository;
        }

        public void AddCustomerLocation(string customerId, int bookingId, double longitude, double latitude)
        {
            var location = new CustomerLocations
            {
                Time = DateTimeUtil.GetTimeNow(),
                CustomerId = customerId,
                BookingId = bookingId,
                GpsLat = latitude,
                GpsLong = longitude
            };

            _locationRepository.Add(location);
        }

        public List<CustomerLocations> GetCustomerLocations(string customerId, int bookingId)
        {
            var locations = _locationRepository.GetAll()
                .Where(l => l.BookingId == bookingId && l.CustomerId == customerId)
                .ToList();

            return locations;
        }

        public double CheckCustomerLocations(string customerId, int bookingId)
        {
            var booking = _bookingRepository.GetAll()
                .Where(b => b.Id == bookingId)
                .Include(b => b.Barber)
                .First();
            var bookedTime = booking.BookedTime;
            var currentTime = DateTimeUtil.GetTimeNow();

            if (currentTime < bookedTime.AddMinutes(booking.DurationMinute * BookingConstants.BarberLatencyPercentage))
            {
                return 0;
            }

            var barber = booking.Barber;
            var locations = GetCustomerLocations(customerId, bookingId)
                .Where(l => bookedTime <= l.Time && l.Time <= currentTime)
                .ToList();
            double customerInPlaceCount = 0;
            foreach (var location in locations)
            {
                var distance = LocationUtil.CalculateDistance(location.GpsLong, location.GpsLat,
                    barber.Longitude.GetValueOrDefault(), barber.Latitude.GetValueOrDefault());
                if (distance <= LocationUtil.AcceptableDistance)
                {
                    customerInPlaceCount += 1;
                }
            }

            return customerInPlaceCount / locations.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <description>
        ///     Reference from https://www.gso.gov.vn/dmhc2015/
        /// </description>
        /// <returns></returns>
        public bool InitCityDistrictDatabase()
        {
            if (_cityRepository.GetAll().ToList().Count != 0)
            {
                return false;
            }
            
            var webRequest = WebRequest.Create(@"https://raw.githubusercontent.com/madnh/hanhchinhvn/master/dist/tinh_tp.json");

            using (var response = webRequest.GetResponse())
            using(var content = response.GetResponseStream())
            using(var reader = new StreamReader(content ?? throw new Exception())){
                var strContent = reader.ReadToEnd();
                Dictionary<string, City> cities = JsonConvert.DeserializeObject<Dictionary<string, City>>(strContent);
                var cityEntities = cities.Select(c => new Cities
                {
                    Code = c.Value.Code,
                    Name = c.Value.Name
                }).ToList();

                _cityRepository.AddBulk(cityEntities);
            }
            
            webRequest = WebRequest.Create(@"https://raw.githubusercontent.com/madnh/hanhchinhvn/master/dist/quan_huyen.json");

            using (var response = webRequest.GetResponse())
            using(var content = response.GetResponseStream())
            using(var reader = new StreamReader(content ?? throw new Exception())){
                var strContent = reader.ReadToEnd();
                Dictionary<string, District> districts = JsonConvert.DeserializeObject<Dictionary<string, District>>(strContent);
                var entities = districts.Select(d=> new Districts
                {
                    Code = d.Value.Code,
                    Name = d.Value.Name,
                    CitiCode = d.Value.Parent_code
                }).ToList();

                _districtRepository.AddBulk(entities);
            }

            return true;
        }
    }
}