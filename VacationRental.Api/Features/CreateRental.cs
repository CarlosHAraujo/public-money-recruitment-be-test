﻿using MediatR;
using VacationRental.Api.Models;
using VacationRental.Core;

namespace VacationRental.Api.Features
{
    public class CreateRental
    {
        public class Command : IRequest<ResourceIdViewModel>
        {
            public Command(int units, int preparationTimeInDays)
            {
                Units = units;
                PreparationTimeInDays = preparationTimeInDays;
            }

            public int Units { get; private set; }

            public int PreparationTimeInDays { get; private set; }
        }

        public class CommandHandler : RequestHandler<Command, ResourceIdViewModel>
        {
            private readonly IRentalRepository _rentals;

            public CommandHandler(IRentalRepository rentals)
            {
                _rentals = rentals;
            }

            protected override ResourceIdViewModel Handle(Command request)
            {
                Rental rental = new Rental
                {
                    Units = request.Units,
                    PreparationTimeInDays = request.PreparationTimeInDays
                };

                int key = _rentals.Create(rental);

                return new ResourceIdViewModel
                {
                    Id = key
                };
            }
        }
    }
}
