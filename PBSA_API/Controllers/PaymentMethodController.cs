using System.Collections.Generic;
using DataService.Models;
using DataService.Services;
using DataService.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DataService.Constants.PaymentConstants;

namespace PBSA_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;

        public PaymentMethodController(IPaymentMethodService paymentMethodService)
        {
            this._paymentMethodService = paymentMethodService;
        }

        [HttpGet("/payment/default")]
        public ActionResult GetDefaultPayment(string userId)
        {
            PaymentMethods paymentMethod = _paymentMethodService.GetUserDefaultPaymentMethod(userId);
            if (paymentMethod == null)
            {
                return new JsonResult(null) { StatusCode = StatusCodes.Status404NotFound };
            }
            PaymentMethodViewModel paymentMethodVM = new PaymentMethodViewModel()
            {
                Id = paymentMethod.Id,
                CreatedTime = paymentMethod.CreatedTime,
                InUsed = paymentMethod.InUsed,
                IsDefault = paymentMethod.IsDefault,
                PaymentType = paymentMethod.PaymentType,
                UserId = paymentMethod.UserId
            };
            if (paymentMethodVM.PaymentType == PaymentType.Wallet)
            {
                paymentMethodVM.Wallets = paymentMethod.Wallets;
            }


            return new JsonResult(paymentMethodVM) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost("/payment/default")]
        public ActionResult SetDefaultPayment(string userId, int paymentMethodId)
        {
            bool result = _paymentMethodService.SetDefaultPaymentMethod(userId, paymentMethodId);
            if (result == false)
            {
                return new JsonResult(null) { StatusCode = StatusCodes.Status404NotFound };
            }
            return new JsonResult(null) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet("/payment/get-all")]
        public ActionResult GetAllUserPayment(string userId)
        {
            List<PaymentMethods> userPaymentMethods = _paymentMethodService.GetAllUserPaymentMethod(userId);

            List<PaymentMethodViewModel> paymentMethodVMs = new List<PaymentMethodViewModel>();
            PaymentMethodViewModel paymentMethodVM;
            foreach (var upm in userPaymentMethods)
            {
                paymentMethodVM = new PaymentMethodViewModel()
                {
                    Id = upm.Id,
                    CreatedTime = upm.CreatedTime,
                    InUsed = upm.InUsed,
                    IsDefault = upm.IsDefault,
                    PaymentType = upm.PaymentType,
                    UserId = upm.UserId,
                };
                if (paymentMethodVM.PaymentType == PaymentType.Wallet)
                {
                    paymentMethodVM.Wallets = upm.Wallets;
                }

                paymentMethodVMs.Add(paymentMethodVM);
            }

            return new JsonResult(paymentMethodVMs) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet("/payment")]
        public ActionResult Payment(string userId, string paymentType)
        {
            PaymentMethods paymentMethod = _paymentMethodService.GetUserPaymentMethodByType(userId, paymentType);
            if (paymentMethod == null)
            {
                return new JsonResult(null) { StatusCode = StatusCodes.Status404NotFound };
            }

            PaymentMethodViewModel paymentMethodVM = new PaymentMethodViewModel()
            {
                Id = paymentMethod.Id,
                CreatedTime = paymentMethod.CreatedTime,
                InUsed = paymentMethod.InUsed,
                IsDefault = paymentMethod.IsDefault,
                PaymentType = paymentMethod.PaymentType,
                UserId = paymentMethod.UserId
            };
            if (paymentMethodVM.PaymentType == PaymentType.Wallet)
            {
                paymentMethodVM.Wallets = paymentMethod.Wallets;
            }

            return new JsonResult(paymentMethodVM) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost("/payment/top-up")]
        public ActionResult TopupMoney(string userId, decimal amount)
        {
            bool result = _paymentMethodService.Topup(userId, amount);
            if (!result)
            {
                return new JsonResult(null) { StatusCode = StatusCodes.Status404NotFound };
            }
            return new JsonResult(null) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost("/payment/withdraw")]
        public ActionResult Withdraw(string userId, int amount)
        {
            bool result = _paymentMethodService.Withdraw(userId, amount);
            if (!result) //Payment failed or Balance is not enough
            {
                return new JsonResult(null) { StatusCode = StatusCodes.Status404NotFound };
            }
            return new JsonResult(null) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost("/payment/payable")]
        public ActionResult Payable(int paymentMethodId, int amount)
        {
            string result = _paymentMethodService.Payable(paymentMethodId, amount);
            if (result == PaymentDescription.NotAvailable)
            {
                return new JsonResult(PaymentDescription.NotAvailable_VN) { StatusCode = StatusCodes.Status404NotFound };
            }
            else if (result == PaymentDescription.NotPayable)
            {
                return new JsonResult(PaymentDescription.NotPayable_VN) { StatusCode = StatusCodes.Status409Conflict };
            }
            else if (result == PaymentDescription.Payable)
            {
                return new JsonResult(PaymentDescription.Payable_VN) { StatusCode = StatusCodes.Status200OK };
            }
            return new JsonResult(PaymentDescription.NotAvailable_VN) { StatusCode = StatusCodes.Status404NotFound };
        }

        [HttpPost("/payment/pay")]
        public ActionResult Pay(string senderId, string receiverId, string paymentType, decimal amount)
        {
            bool result = _paymentMethodService.Pay(senderId, receiverId, paymentType, amount);
            if (!result)
            {
                return new JsonResult(PaymentDescription.NotEnoughMoney_VN) { StatusCode = StatusCodes.Status409Conflict };
            }
            return new JsonResult(null) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost("/payment/make-deposit")]
        public ActionResult MakeDeposit(string senderId, string receiverId, decimal amount, int bookingId)
        {
            bool result = _paymentMethodService.MakeDeposit(senderId, receiverId, amount, bookingId);
            if (!result)
            {
                return new JsonResult(PaymentDescription.NotEnoughMoney_VN) { StatusCode = StatusCodes.Status409Conflict };
            }
            return new JsonResult(null) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost("/payment/take-away-deposit")]
        public ActionResult TakeAwayDeposit(int transactionId)
        {
            bool result = _paymentMethodService.TakeAwayDeposit(transactionId);
            if (!result)
            {
                return new JsonResult(PaymentDescription.NotSuccess_VN) { StatusCode = StatusCodes.Status409Conflict };
            }
            return new JsonResult(null) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost("/payment/return-deposit")]
        public ActionResult ReturnDeposit(int transactionId)
        {
            bool result = _paymentMethodService.ReturnDeposit(transactionId);
            if (!result)
            {
                return new JsonResult(PaymentDescription.NotSuccess_VN) { StatusCode = StatusCodes.Status409Conflict };
            }
            return new JsonResult(null) { StatusCode = StatusCodes.Status200OK };
        }
    }
}