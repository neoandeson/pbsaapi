using DataService.Models;
using DataService.Repositories;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DataService.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataService.Services
{
    public interface IFeedbackService
    {
        List<FeedbackViewModel> GetBarberFeedbacks(string barberId);
        bool AddFeedBack(int bookingId, float rate, string comment);
    }

    public class FeedbackService : IFeedbackService
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBarberRepository _barberRepository;

        public FeedbackService(IMapper mapper, IBookingRepository bookingRepository, IBarberRepository barberRepository)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _barberRepository = barberRepository;
        }

        public bool AddFeedBack(int bookingId, float rate, string comment)
        {
            Bookings booking = _bookingRepository.GetById(bookingId);
            if (booking.Rate != null)
            {
                return false;
            }

            booking.Rate = rate;
            booking.Comment = comment;
            _bookingRepository.Update(booking);

            Barbers barber = _barberRepository
                .GetAll()
                .Where(b => b.UserId == booking.BarberId)
                .Include(b => b.Bookings)
                .First();
            var totalRates = barber.Bookings.Sum(barberBooking => barberBooking.Rate).GetValueOrDefault();
            var totalCount = barber.Bookings.Count(barberBooking => barberBooking.Rate != null);
            barber.OverallRate = totalRates / totalCount;
            barber.RatingCount = totalCount;
            _barberRepository.Update(barber);

            return true;
        }

        public List<FeedbackViewModel> GetBarberFeedbacks(string barberId)
        {
            var result = _bookingRepository
                .GetAll()
                .Where(booking => booking.BarberId == barberId && booking.Rate != null)
                .Include(booking => booking.Customer)
                .Select(booking => new FeedbackViewModel
                {
                    Rate = booking.Rate,
                    Comment = booking.Comment,
                    Time = booking.BookedTime,
                    Customer = booking.Customer.FirstName
                }).ToList();

            return result;
        }
    }
}
