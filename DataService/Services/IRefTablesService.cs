using System.Linq;
using DataService.Constants;
using DataService.Models;
using DataService.Repositories;

namespace DataService.Services
{
    public interface IRefTablesService
    {
        bool InitBookingStateDatabase();
        bool InitComplaintTypeDatabase();
        bool InitComplaintStateDatabase();
        bool InitCurrencyCodeDatabase();
        bool InitPaymentTypeDatabase();
        bool InitTransactionStateDatabase();
    }

    public class RefTablesService : IRefTablesService
    {
        private readonly IRefBookingStateRepository _bookingStateRepository;
        private readonly IRefComplaintTypeRepository _complaintTypeRepository;
        private readonly IRefComplaintStateRepository _complaintStateRepository;
        private readonly IRefCurrencyCodeRepository _currencyCodeRepository;
        private readonly IRefPaymentTypeRepository _paymentTypeRepository;
        private readonly IRefTransactionStateRepository _transactionStateRepository;

        public RefTablesService(IRefBookingStateRepository bookingStateRepository, 
            IRefComplaintTypeRepository complaintTypeRepository, IRefComplaintStateRepository complaintStateRepository, 
            IRefCurrencyCodeRepository currencyCodeRepository, IRefPaymentTypeRepository paymentTypeRepository, 
            IRefTransactionStateRepository transactionStateRepository)
        {
            _bookingStateRepository = bookingStateRepository;
            _complaintTypeRepository = complaintTypeRepository;
            _complaintStateRepository = complaintStateRepository;
            _currencyCodeRepository = currencyCodeRepository;
            _paymentTypeRepository = paymentTypeRepository;
            _transactionStateRepository = transactionStateRepository;
        }
        
        public bool InitBookingStateDatabase()
        {
            foreach (var state in BookingConstants.States)
            {
                var bookingState = new RefBookingStates
                {
                    Code = state,
                    Name = state
                };
                _bookingStateRepository.AddOrUpdate(s => s.Code == bookingState.Code, bookingState);
            }

            return true;
        }

        public bool InitComplaintTypeDatabase()
        {
            foreach (var type in ComplaintConstants.ComplaintType.Array)
            {
                var complaintType = new RefComplaintTypes
                {
                    Code = type,
                    Name = type
                };
                _complaintTypeRepository.AddOrUpdate(s => s.Code == complaintType.Code, complaintType);
            }

            return true;
        }

        public bool InitComplaintStateDatabase()
        {
            foreach (var type in ComplaintConstants.ComplaintState.Array)
            {
                var complaintState = new RefComplaintStates
                {
                    Code = type,
                    Name = type
                };
                _complaintStateRepository.AddOrUpdate(s => s.Code == complaintState.Code, complaintState);
            }

            return true;
        }

        public bool InitCurrencyCodeDatabase()
        {
            foreach (var type in CurrencyConstants.Array)
            {
                var currencyCode = new RefCurrencyCodes
                {
                    Code = type,
                    Name = type
                };
                _currencyCodeRepository.AddOrUpdate(s => s.Code == currencyCode.Code, currencyCode);
            }

            return true;
        }

        public bool InitPaymentTypeDatabase()
        {
            foreach (var type in PaymentConstants.PaymentType.Array)
            {
                var paymentType = new RefPaymentTypes
                {
                    Code = type,
                    Name = type
                };
                _paymentTypeRepository.AddOrUpdate(s => s.Code == paymentType.Code, paymentType);
            }

            return true;
        }

        public bool InitTransactionStateDatabase()
        {
            foreach (var type in TransactionConstants.TransactionState.Array)
            {
                var transactionState = new RefTransactionStates
                {
                    Code = type,
                    Name = type
                };
                _transactionStateRepository.AddOrUpdate(s => s.Code == transactionState.Code, transactionState);
            }

            return true;
        }
    }
}