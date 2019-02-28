using System.Collections.Generic;
using System.Linq;
using DataService.Constants;
using DataService.Infrastructure;
using DataService.Models;
using DataService.Utils;

namespace DataService.Repositories
{
    public interface IPaymentMethodRepository : IRepository<PaymentMethods>
    {
        PaymentMethods GetDefaultPaymentMethod(string userId);
        bool CreatePaymentMethodsForNewUser(string userId);
    }

    public class PaymentMethodRepository : Repository<PaymentMethods>, IPaymentMethodRepository
    {
        
        public PaymentMethodRepository(PBSAContext context) : base(context)
        {
        }

        public PaymentMethods GetDefaultPaymentMethod(string userId)
        {
            var list =  GetAll()
                .Where(method => method.InUsed
                                 && method.UserId == userId
                                 && method.IsDefault)
                .ToList();

            return list.Count == 0 ? null : list[0];
        }

        public bool CreatePaymentMethodsForNewUser(string userId)
        {
            PaymentMethods cashMethod = new PaymentMethods
            {
                UserId = userId,
                CreatedTime = DateTimeUtil.GetTimeNow(),
                InUsed = true,
                IsDefault = false,
                PaymentType = PaymentConstants.PaymentType.Cash
            };
            Add(cashMethod);
            
            PaymentMethods walletMethods = new PaymentMethods
            {
                UserId = userId,
                CreatedTime = DateTimeUtil.GetTimeNow(),
                InUsed = true,
                IsDefault = true,
                PaymentType = PaymentConstants.PaymentType.Wallet
            };
            var walletPaymentMethod = Add(walletMethods);
            
            Wallets wallet = new Wallets
            {
                Id = walletPaymentMethod.Id,
                UserId = userId,
                Balance = 0,
                CurrencyCode = CurrencyConstants.VND,
                CreatedTime = DateTimeUtil.GetTimeNow(),
                InUsed = true
            };
            AddAny(wallet);

            return true;
        }
    }
}