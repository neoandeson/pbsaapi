using DataService.Models;
using DataService.Repositories;
using System.Collections.Generic;
using System.Linq;
using DataService.Constants;
using DataService.Utils;
using Microsoft.EntityFrameworkCore;

namespace DataService.Services
{
    public interface IBarberService
    {
        List<Barbers> GetAll();
        List<Barbers> GetBarbersByCity(string cityCode);
        List<Barbers> GetBarbersByName(string name);
        Barbers GetBarberInfo(string username);
        List<BarberServices> GetServicesByBarber(string barberId);
        Barbers RegisterBarber(string username, string fullName, string phone);
        BarberServices AddOrUpdateBarberService(BarberServices service);
        bool RemoveBarberService(int serviceId);
        void Save(Barbers barber);
    }

    public class BarberService : IBarberService
    {
        private readonly IAspNetUserRepository _aspNetUserRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBarberRepository _barberRepository;
        private readonly IBarberServiceRepository _serviceRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public BarberService(IBarberRepository barberRepository, IBarberServiceRepository serviceRepository,
            IAspNetUserRepository aspNetUserRepository, ICustomerRepository customerRepository,
            IPaymentMethodRepository paymentMethodRepository)
        {
            _barberRepository = barberRepository;
            _serviceRepository = serviceRepository;
            _aspNetUserRepository = aspNetUserRepository;
            _customerRepository = customerRepository;
            _paymentMethodRepository = paymentMethodRepository;
        }

        public List<Barbers> GetBarbersByCity(string cityCode)
        {
            List<Barbers> barbers = _barberRepository.GetAll().Where(b => b.CityCode == cityCode).ToList();
            return barbers;
        }

        public List<Barbers> GetBarbersByName(string name)
        {
            List<Barbers> barbers = _barberRepository.GetAll().Where(b => b.FullName.Contains(name)).ToList();
            return barbers;
        }

        public Barbers GetBarberInfo(string username)
        {
            Barbers barber = _barberRepository
                .GetAll()
                .Where(b => b.Username == username)
                .Include(b => b.CityCodeNavigation)
                .Include(b => b.DistrictCodeNavigation)
                .Include(b => b.User)
                    .ThenInclude(user => user.PaymentMethods)
                .First();
            return barber;
        }

        public List<BarberServices> GetServicesByBarber(string barberId)
        {
            return _serviceRepository.GetServicesByBarber(barberId);
        }

        public Barbers RegisterBarber(string username, string fullName, string phone)
        {
            AspNetUsers user = _aspNetUserRepository.RegisterUser(username, fullName, phone);

            if (user == null) return null;

            Barbers barber = new Barbers
            {
                UserId = user.Id,
                Username = user.UserName,
                FullName = user.FullName,
                Address = "address",
                ContactPhone = phone,
                CityCode = "79",
                DistrictCode = "760",
                IsActive = true
            };
            var rs = _barberRepository.Add(barber);

            //Create a stub user for barber
            Customers customer = new Customers
            {
                UserId = user.Id,
                Username = user.UserName,
                Address = "address",
                FirstName = user.FullName,
                IsActive = true
            };
            _customerRepository.Add(customer);

            //Create payment methods
            _paymentMethodRepository.CreatePaymentMethodsForNewUser(user.Id);

            return rs;
        }

        public BarberServices AddOrUpdateBarberService(BarberServices service)
        {
            BarberServices currentService = _serviceRepository.GetById(service.Id);
            if (currentService != null)
            {
                if (currentService.EqualsTo(service))
                {
                    //When nothing changed
                    return currentService;
                }
                RemoveBarberService(service.Id);
            }

            service.Id = default(int);
            var rs = _serviceRepository.Add(service);

            return rs;
        }

        public bool RemoveBarberService(int serviceId)
        {
            BarberServices service = _serviceRepository.GetById(serviceId);
            if (service != null)
            {
                service.InUsed = false;
                service.ExpiredTime = DateTimeUtil.GetTimeNow();
                _serviceRepository.Update(service);
                return true;
            }

            return false;
        }

        public List<Barbers> GetAll()
        {
            List<Barbers> barbers = _barberRepository.GetAll().ToList();
            return barbers;
        }

        public void Save(Barbers barber)
        {
            _barberRepository.Update(barber);
        }
    }
}
