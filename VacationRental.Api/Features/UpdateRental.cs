using MediatR;
using System;
using VacationRental.Core;

namespace VacationRental.Api.Features
{
    public class UpdateRental
    {
        public class Command : IRequest
        {
            public Command(int id, int units, int preparationTimeInDays)
            {
                Id = id;
                Units = units;
                PreparationTimeInDays = preparationTimeInDays;
            }

            public int Id { get; private set; }

            public int Units { get; private set; }

            public int PreparationTimeInDays { get; private set; }
        }

        public class CommandHandler : RequestHandler<Command>
        {
            private readonly IRentalRepository _rentals;
            private readonly IAvailabilityService _availabilityService;
            private readonly IBookingRepository _bookings;

            public CommandHandler(
                IRentalRepository rentals,
                IAvailabilityService availabilityService,
                IBookingRepository bookings)
            {
                _rentals = rentals;
                _availabilityService = availabilityService;
                _bookings = bookings;
            }

            protected override void Handle(Command request)
            {
                if(request.Units <= 0)
                    throw new ApplicationException("Units must be positive");

                Rental rental = _rentals.Get(request.Id);

                if (rental is null)
                    throw new ApplicationException("Rental not found");

                rental.Bookings = _bookings.GetByRentalId(rental.Id);
                
                if ((request.Units < rental.Units) 
                    && !_availabilityService.CheckCapacityForUnitDecrease(rental, request.Units))
                    throw new ApplicationException("There are bookings that overflow the desired update unit capacity");

                if ((request.PreparationTimeInDays > rental.PreparationTimeInDays) 
                    && !_availabilityService.CheckCapacityForPreparationTimeIncrease(rental, request.PreparationTimeInDays))
                    throw new ApplicationException("There are bookings that overflow the desired update preparation time");

                Rental newRental = new Rental
                {
                    Id = rental.Id,
                    Units = request.Units,
                    PreparationTimeInDays = request.PreparationTimeInDays,
                    Bookings = rental.Bookings
                };

                _rentals.Update(newRental);
            }
        }
    }
}
