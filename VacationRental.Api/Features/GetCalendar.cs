using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Core;

namespace VacationRental.Api.Features
{
    public class GetCalendar
    {
        public class Query : IRequest<CalendarViewModel>
        {
            public Query(int rentalId, DateTime start, int nights)
            {
                RentalId = rentalId;
                Start = start;
                Nights = nights;
            }

            public int Nights { get; private set; }

            public DateTime Start { get; private set; }

            public int RentalId { get; private set; }
        }

        public class QueryHandler : RequestHandler<Query, CalendarViewModel>
        {
            private readonly IRentalRepository _rentals;
            private readonly IAvailabilityService _availabilityService;

            public QueryHandler(IRentalRepository rentals, IAvailabilityService availabilityService)
            {
                _rentals = rentals;
                _availabilityService = availabilityService;
            }

            protected override CalendarViewModel Handle(Query request)
            {
                if (request.Nights < 0)
                    throw new ApplicationException("Nights must be positive");

                Rental rental = _rentals.Get(request.RentalId);

                if (rental is null)
                    throw new ApplicationException("Rental not found");

                var dates = _availabilityService.GetCalendarDates(rental, request.Start, request.Nights);

                var result = new CalendarViewModel
                {
                    RentalId = request.RentalId,
                    Dates = new List<CalendarDateViewModel>(dates.Select(calendarDay => new CalendarDateViewModel
                    {
                        Date = calendarDay.Date,
                        Bookings = new List<CalendarBookingViewModel>(calendarDay.Bookings.Select(booking => new CalendarBookingViewModel
                        {
                            Id = booking.Id,
                            Unit = booking.Unit
                        })),
                        PreparationTimes = new List<CalendarPreparationTimeViewModel>(calendarDay.PreparationTimes.Select(preparation => new CalendarPreparationTimeViewModel
                        {
                            Unit = preparation.Unit
                        }))
                    }))
                };

                return result;
            }
        }
    }
}
