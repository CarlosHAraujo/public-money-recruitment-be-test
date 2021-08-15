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

            public CommandHandler(IRentalRepository rentals)
            {
                _rentals = rentals;
            }

            protected override void Handle(Command request)
            {
                Rental rental = _rentals.Get(request.Id);

                if (rental is null)
                    throw new ApplicationException("Rental not found");

                if(request.Units < rental.Units)
                    throw new ApplicationException("Functionality to decrease Rental units is not ready yet");

                if(request.PreparationTimeInDays > rental.PreparationTimeInDays)
                    throw new ApplicationException("Functionality to increase Rental preparation time is not ready yet");

                Rental newRental = new Rental
                {
                    Id = rental.Id,
                    Units = request.Units,
                    PreparationTimeInDays = request.PreparationTimeInDays
                };

                _rentals.Update(newRental);
            }
        }
    }
}
