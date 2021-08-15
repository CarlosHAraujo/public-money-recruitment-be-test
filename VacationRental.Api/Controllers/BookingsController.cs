using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Features;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public Task<BookingViewModel> Get(int bookingId)
        {
            return _mediator.Send(new GetBooking.Query(bookingId));
        }

        [HttpPost]
        public Task<ResourceIdViewModel> Post(BookingBindingModel model)
        {
            return _mediator.Send(new CreateBooking.Command(model.RentalId, model.Nights, model.Start));
        }
    }
}
