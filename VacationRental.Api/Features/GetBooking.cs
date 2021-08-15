using MediatR;
using System;
using VacationRental.Api.Models;
using VacationRental.Core;

namespace VacationRental.Api.Features
{
    public class GetBooking
    {
        public class Query : IRequest<BookingViewModel>
        {
            public Query(int id)
            {
                Id = id;
            }

            public int Id { get; private set; }
        }

        public class QueryHandler : RequestHandler<Query, BookingViewModel>
        {
            private readonly IBookingRepository _bookings;

            public QueryHandler(IBookingRepository bookings)
            {
                _bookings = bookings;
            }

            protected override BookingViewModel Handle(Query request)
            {
                var booking = _bookings.Get(request.Id);

                if (booking is null)
                    throw new ApplicationException("Booking not found");

                return new BookingViewModel
                {
                    Id = booking.Id,
                    Nights = booking.Nights,
                    RentalId = booking.RentalId,
                    Start = booking.Start,
                    Unit = booking.Unit
                };
            }
        }
    }
}
