using DataService.Models;
using DataService.Repositories;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataService.Services
{
    public interface ICustomerService
    {
        Customers RegisterCustomer(string username, string firstName, string middleName, string lastName, string phone);
        Customers GetCustomerInfo(string username);
        List<Customers> GetAll();
        void Save(Customers customer);
    }

    public class CustomerService : ICustomerService
    {
        private readonly IAspNetUserRepository _aspNetUserRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public CustomerService(IAspNetUserRepository aspNetUserRepository, ICustomerRepository customerRepository, IPaymentMethodRepository paymentMethodRepository)
        {
            _aspNetUserRepository = aspNetUserRepository;
            _customerRepository = customerRepository;
            _paymentMethodRepository = paymentMethodRepository;
        }

        public List<Customers> GetAll()
        {
            List<Customers> rs = _customerRepository
                .GetAll()
                .Include(c => c.User)
                .ToList();
            return rs;
        }

        public Customers GetCustomerInfo(string username)
        {
            Customers customer = _customerRepository.GetAll()
                .Where(u => u.Username == username)
                .Include(c => c.User.PaymentMethods)
                .FirstOrDefault();
            return customer;
        }

        public Customers RegisterCustomer(string username, string firstName, string middleName, string lastName, string phone)
        {
            AspNetUsers user = _aspNetUserRepository.RegisterUser(username, firstName, phone);

            if (user == null) return null;
            
            Customers customer = new Customers
            {
                UserId = user.Id,
                Username = user.UserName,
                Address = "address",
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                IsActive = true
            };
            var rs = _customerRepository.Add(customer);
            
            //Create payment methods
            _paymentMethodRepository.CreatePaymentMethodsForNewUser(user.Id);

            return rs;
        }

        public void Save(Customers customer)
        {
            _customerRepository.Update(customer);
        }
    }
}