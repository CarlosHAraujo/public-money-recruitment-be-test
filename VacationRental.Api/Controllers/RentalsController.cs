using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Features;
using VacationRental.Api.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RentalsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public Task<RentalViewModel> Get(int rentalId)
        {
            return _mediator.Send(new GetRental.Query(rentalId));
        }

        [HttpPost]
        public Task<ResourceIdViewModel> Post(RentalBindingModel model)
        {
            return _mediator.Send(new CreateRental.Command(model.Units, model.PreparationTimeInDays));
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public async Task<NoContentResult> Put(int rentalId, RentalBindingModel model)
        {
            await _mediator.Send(new UpdateRental.Command(rentalId, model.Units, model.PreparationTimeInDays));

            return NoContent();
        }
    }
}
