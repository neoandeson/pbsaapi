using DataService.Constants;
using DataService.Models;
using DataService.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataService.Utils;
using static DataService.Constants.PaymentConstants;
using static DataService.Constants.TransactionConstants;

namespace DataService.Services
{
    public interface IPaymentMethodService
    {
        bool SetDefaultPaymentMethod(string userId, int paymentMethodId);
        List<PaymentMethods> GetAllUserPaymentMethod(string userId);
        PaymentMethods GetUserDefaultPaymentMethod(string userId);
        PaymentMethods GetUserPaymentMethodByType(string userId, string paymentType);
        bool Topup(string userId, decimal amount);
        bool Withdraw(string userId, decimal amount);
        string Payable(int paymentMethodId, decimal amount);
        bool Pay(string senderId, string receiverId, string paymentType, decimal amount);
        bool ForcePay(string senderId, string receiverId, string paymentType, decimal amount, int bookingId,
            string detail);
        bool MakeDeposit(string senderId, string receiverId, decimal amount, int bookingId);
        bool TakeAwayDeposit(int transactionId);
        bool ReturnDeposit(int transactionId);
        List<Transactions> GetDepositTransactionsByBooking(int bookingId);
    }

    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;

        public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository, IWalletRepository walletRepository, ITransactionRepository transactionRepository)
        {
            this._paymentMethodRepository = paymentMethodRepository;
            this._walletRepository = walletRepository;
            this._transactionRepository = transactionRepository;
        }

        public List<PaymentMethods> GetAllUserPaymentMethod(string userId)
        {
            List<PaymentMethods> paymentMethods = _paymentMethodRepository
                .GetAll()
                .Where(pm => pm.UserId == userId)
                .Include(pm => pm.Wallets)
                .ToList();
            return paymentMethods;
        }

        public PaymentMethods GetUserDefaultPaymentMethod(string userId)
        {
            PaymentMethods paymentMethods = _paymentMethodRepository
                .GetAll()
                .Where(pm => pm.UserId == userId && pm.IsDefault == true)
                .Include(pm => pm.Wallets)
                .FirstOrDefault();
            return paymentMethods;
        }

        public PaymentMethods GetUserPaymentMethodByType(string userId, string paymentType)
        {
            PaymentMethods paymentMethods = _paymentMethodRepository
                .GetAll()
                .Where(pm => pm.UserId == userId && pm.PaymentType == paymentType)
                .Include(pm => pm.User)
                .Include(pm => pm.Wallets)
                .FirstOrDefault();
            return paymentMethods;
        }

        public bool ForcePay(string senderId, string receiverId, string paymentType, decimal amount, 
            int bookingId, string detail)
        {
            try
            {
                PaymentMethods senderMethod = GetUserPaymentMethodByType(senderId, paymentType);
                PaymentMethods receiverMethod = GetUserPaymentMethodByType(receiverId, paymentType);

                Transactions transaction = new Transactions
                {
                    Amount = amount,
                    CurrencyCode = CurrencyConstants.VND,
                    Detail = detail,
                    State = TransactionState.Completed,
                    SenderName = senderMethod.User.FullName,
                    SenderPaymentMethodId = senderMethod.Id,
                    ReceiverName = receiverMethod.User.FullName,
                    ReceiverPaymentMethodId = receiverMethod.Id,
                    Time = DateTimeUtil.GetTimeNow(),
                    BookingId = bookingId 
                };
                _transactionRepository.Add(transaction);

                if (paymentType == PaymentType.Wallet)
                {
                    senderMethod.Wallets.Balance -= amount;
                    receiverMethod.Wallets.Balance += amount;
                    _walletRepository.Update(senderMethod.Wallets);
                    _walletRepository.Update(receiverMethod.Wallets);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool MakeDeposit(string senderId, string receiverId, decimal amount, int bookingId)
        {
            try
            {
                Wallets senderWallet = _walletRepository.GetAll().Include(w => w.IdNavigation.User).Where(w => w.UserId == senderId).FirstOrDefault();
                Wallets receiveWallet = _walletRepository.GetAll().Include(w => w.IdNavigation.User).Where(w => w.UserId == receiverId).FirstOrDefault();

                // create deposit transaction for hold deposit
                Transactions depositTransaction = new Transactions()
                {
                    Amount = amount,
                    CurrencyCode = CurrencyConstants.VND,
                    Detail = TransactionDetail.DepositTransaction,
                    State = TransactionState.TemporaryDeposit,
                    BookingId = bookingId,
                    SenderName = senderWallet.IdNavigation.User.FullName,
                    SenderPaymentMethodId = senderWallet.IdNavigation.Id,
                    ReceiverName = receiveWallet.IdNavigation.User.FullName,
                    ReceiverPaymentMethodId = receiveWallet.IdNavigation.Id,
                    Time = DateTime.Now
                };
                _transactionRepository.Add(depositTransaction);

                //Minute amount in sender' wallet
                senderWallet.Balance = senderWallet.Balance - amount;
                _walletRepository.Update(senderWallet);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool Pay(string senderId, string receiverId, string paymentType, decimal amount)
        {
            try
            {
                PaymentMethods senderPM = GetUserPaymentMethodByType(senderId, paymentType);
                PaymentMethods receiverPM = GetUserPaymentMethodByType(receiverId, paymentType);

                if (paymentType == PaymentType.Wallet)
                {
                    if (senderPM != null && receiverPM != null)
                    {
                        //Check balance after transaction is more than minimum balance
                        if (senderPM.Wallets.Balance - amount < PaymentAmount.MinimumBalance)
                        {
                            return false;
                        }

                        Transactions depositTransaction = new Transactions()
                        {
                            Amount = amount,
                            CurrencyCode = CurrencyConstants.VND,
                            Detail = TransactionDetail.PaidTransaction,
                            State = TransactionState.Completed,
                            SenderName = senderPM.User.FullName,
                            SenderPaymentMethodId = senderPM.Wallets.IdNavigation.Id,
                            ReceiverName = receiverPM.User.FullName,
                            ReceiverPaymentMethodId = receiverPM.Wallets.IdNavigation.Id,
                            Time = DateTime.Now
                        };
                        _transactionRepository.Add(depositTransaction);

                        senderPM.Wallets.Balance = senderPM.Wallets.Balance - amount;
                        receiverPM.Wallets.Balance = receiverPM.Wallets.Balance + amount;
                        _walletRepository.Update(senderPM.Wallets);
                        _walletRepository.Update(receiverPM.Wallets);
                    }
                }
                else if (paymentType == PaymentType.Cash)
                {
                    Transactions depositTransaction = new Transactions()
                    {
                        Amount = amount,
                        CurrencyCode = CurrencyConstants.VND,
                        Detail = TransactionDetail.PaidTransaction,
                        State = TransactionState.Completed,
                        SenderName = senderPM.User.FullName,
                        SenderPaymentMethodId = senderPM.Id,
                        ReceiverName = receiverPM.User.FullName,
                        ReceiverPaymentMethodId = receiverPM.Id,
                        Time = DateTime.Now
                    };
                    _transactionRepository.Add(depositTransaction);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public string Payable(int paymentMethodId, decimal amount)
        {
            PaymentMethods method = _paymentMethodRepository
                .GetAll()
                .Where(pm => pm.Id == paymentMethodId)
                .Include(pm => pm.Wallets)
                .FirstOrDefault();

            if (method != null)
            {
                if (method.PaymentType == PaymentType.Wallet)
                {
                    Wallets wallet = method.Wallets;
                    if (wallet.Balance - amount >= PaymentAmount.MinimumBalance)
                    {
                        return PaymentDescription.Payable;
                    }

                    return PaymentDescription.NotPayable;
                }
            }

            return PaymentDescription.NotAvailable;
        }

        public bool ReturnDeposit(int transactionId)
        {
            try
            {
                Transactions depositTransaction = _transactionRepository
                    .GetAll()
                    .Where(t => t.Id == transactionId)
                    .Include(t => t.SenderPaymentMethod)
                        .ThenInclude(t => t.Wallets)
                    .FirstOrDefault();

                if (depositTransaction.State == TransactionState.TemporaryDeposit)
                {
                    Wallets senderWallet = depositTransaction.SenderPaymentMethod.Wallets;

                    // Take deposit amount to receiver' wallet
                    senderWallet.Balance = senderWallet.Balance + depositTransaction.Amount;
                    _walletRepository.Update(senderWallet);

                    //Delete transaction
                    _transactionRepository.Delete(depositTransaction);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Transactions> GetDepositTransactionsByBooking(int bookingId)
        {
            List<Transactions> transactions = _transactionRepository.GetAll()
                .Where(transaction => 
                    transaction.BookingId == bookingId
                    && transaction.State == TransactionState.TemporaryDeposit)
                .ToList();
            
            return transactions;
        }

        public bool SetDefaultPaymentMethod(string userId, int paymentMethodId)
        {
            bool isContainPM = false;
            try
            {
                List<PaymentMethods> userPaymentMethods = _paymentMethodRepository.GetAll().Where(pm => pm.UserId == userId).ToList();
                foreach (var upm in userPaymentMethods)
                {
                    if (upm.Id == paymentMethodId)
                    {
                        isContainPM = true;
                        upm.IsDefault = true;
                    }
                    else
                    {
                        upm.IsDefault = false;
                    }
                    _paymentMethodRepository.Update(upm);
                }
                return isContainPM;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TakeAwayDeposit(int transactionId)
        {
            try
            {
                Transactions depositTransaction = _transactionRepository
                    .GetAll()
                    .Where(t => t.Id == transactionId)
                    .Include(t => t.ReceiverPaymentMethod)
                        .ThenInclude(t => t.Wallets)
                    .FirstOrDefault();

                if (depositTransaction.State == TransactionState.TemporaryDeposit)
                {
                    Wallets receiveWallet = depositTransaction.ReceiverPaymentMethod.Wallets;

                    // Take deposit amount to receiver' wallet
                    receiveWallet.Balance = receiveWallet.Balance + depositTransaction.Amount;
                    _walletRepository.Update(receiveWallet);

                    //Change state
                    depositTransaction.State = TransactionState.Completed;
                    depositTransaction.Detail = TransactionDetail.CustomerCompensationTransaction;
                    _transactionRepository.Update(depositTransaction);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Topup(string userId, decimal amount)
        {
            try
            {
                Wallets userWallet = _walletRepository
                    .GetAll()
                    .Where(w => w.UserId == userId)
                    .Include(w => w.IdNavigation)
                    .FirstOrDefault();

                PaymentMethods userCashPM = GetUserPaymentMethodByType(userId, PaymentType.Cash);
                PaymentMethods userWalletPM = GetUserPaymentMethodByType(userId, PaymentType.Wallet);

                if (userWallet != null)
                {
                    userWallet.Balance = userWallet.Balance + amount;
                    _walletRepository.Update(userWallet);

                    Transactions topupTransaction = new Transactions()
                    {
                        Amount = amount,
                        CurrencyCode = CurrencyConstants.VND,
                        Detail = TransactionDetail.TopupTransaction,
                        State = TransactionState.Completed,
                        SenderName = userCashPM.User.FullName,
                        SenderPaymentMethodId = userCashPM.Id,
                        ReceiverName = userWalletPM.User.FullName,
                        ReceiverPaymentMethodId = userWalletPM.Id,
                        Time = DateTime.Now
                    };
                    _transactionRepository.Add(topupTransaction);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool Withdraw(string userId, decimal amount)
        {
            try
            {
                Wallets userWallet = _walletRepository
                    .GetAll()
                    .Where(w => w.UserId == userId)
                    .Include(w => w.IdNavigation)
                    .FirstOrDefault();
                if (userWallet != null)
                {
                    PaymentMethods userCashPM = GetUserPaymentMethodByType(userId, PaymentType.Cash);
                    PaymentMethods userWalletPM = GetUserPaymentMethodByType(userId, PaymentType.Wallet);

                    if (userWallet.Balance - amount >= PaymentAmount.MinimumBalance)
                    {
                        userWallet.Balance = userWallet.Balance - amount;
                        _walletRepository.Update(userWallet);

                        Transactions withdrawTransaction = new Transactions()
                        {
                            Amount = amount,
                            CurrencyCode = CurrencyConstants.VND,
                            Detail = TransactionDetail.WithdrawTransaction,
                            State = TransactionState.Completed,
                            SenderName = userWalletPM.User.FullName,
                            SenderPaymentMethodId = userWalletPM.Id,
                            ReceiverName = userCashPM.User.FullName,
                            ReceiverPaymentMethodId = userCashPM.Id,
                            Time = DateTime.Now
                        };
                        _transactionRepository.Add(withdrawTransaction);
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
