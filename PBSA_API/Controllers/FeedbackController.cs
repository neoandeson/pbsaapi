using System.Collections.Generic;
using DataService.Services;
using DataService.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PBSA_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            this._feedbackService = feedbackService;
        }

        [HttpGet]
        public List<FeedbackViewModel> GetBarberFeedbacks(string username)
        {
            List<FeedbackViewModel> feedbacks = _feedbackService.GetBarberFeedbacks(username);
            return feedbacks;
        }

        [HttpPost]
        public ActionResult AddFeedback(
            [FromQuery] int bookingId, 
            [FromQuery] float rate, 
            [FromQuery] string comment)
        {
            bool rs = _feedbackService.AddFeedBack(bookingId, rate, comment);
            if (!rs)
            {
                return new JsonResult("Something went wrong") { StatusCode = StatusCodes.Status404NotFound };
            }
            return new JsonResult("") { StatusCode = StatusCodes.Status200OK };
        }
    }
}