using MediatR;
using System;
using System.Collections.Generic;
using VacationRental.Api.Models;
using VacationRental.Core;

namespace VacationRental.Api.Features
{
    public class CreateBooking
    {
        public class Command : IRequest<ResourceIdViewModel>
        {
            public Command(int rentalId, int nights, DateTime start)
            {
                RentalId = rentalId;
                Nights = nights;
                Start = start;
            }

            public int RentalId { get; private set; }

            public int Nights { get; private set; }
            
            public DateTime Start { get; private set; }
        }

        public class CommandHandler : RequestHandler<Command, ResourceIdViewModel>
        {
            private readonly IBookingRepository _bookings;
            private readonly IRentalRepository _rentals;
            private readonly IAvailabilityService _availabilityService;

            public CommandHandler(IBookingRepository bookings, IRentalRepository rentals, IAvailabilityService availabilityService)
            {
                _bookings = bookings;
                _rentals = rentals;
                _availabilityService = availabilityService;
            }

            protected override ResourceIdViewModel Handle(Command request)
            {
                if (request.Nights <= 0)
                    throw new ApplicationException("Nigts must be positive");

                Rental rental = _rentals.Get(request.RentalId);

                if (rental is null)
                    throw new ApplicationException("Rental not found");

                List<Booking> bookings = _bookings.GetByRentalId(rental.Id);

                int unitAvailable = _availabilityService.CheckAvailability(rental, request.Start, request.Nights, bookings);

                Booking booking = new Booking
                {
                    Nights = request.Nights,
                    Start = request.Start,
                    RentalId = request.RentalId,
                    Unit = unitAvailable
                };

                int key = _bookings.Create(booking);

                return new ResourceIdViewModel
                {
                    Id = key
                };
            }
        }
    }
}
