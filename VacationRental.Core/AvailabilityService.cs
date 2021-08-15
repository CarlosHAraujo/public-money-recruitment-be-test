using System;
using System.Collections.Generic;
using System.Linq;

namespace VacationRental.Core
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IBookingRepository _bookings;

        public AvailabilityService(IBookingRepository bookings)
        {
            _bookings = bookings;
        }

        public int CheckAvailability(Rental rental, DateTime startDate, int nights)
        {
            HashSet<int> occupiedUnits = new HashSet<int>();

            for (var i = 0; i < nights; i++)
            {
                var count = 0;

                foreach (var booking in rental.Bookings)
                {
                    DateTime bookingEndDate = booking.Start.AddDays(booking.Nights + rental.PreparationTimeInDays);
                    DateTime requestEndDate = startDate.AddDays(nights + rental.PreparationTimeInDays);
                    if ((booking.Start <= startDate.Date && bookingEndDate > startDate.Date)
                        || (booking.Start < requestEndDate && bookingEndDate >= requestEndDate)
                        || (booking.Start > startDate && bookingEndDate < requestEndDate))
                    {
                        count++;
                        occupiedUnits.Add(booking.Unit);
                    }
                }
                if (count >= rental.Units)
                    throw new ApplicationException("Not available");
            }

            return Enumerable.Range(1, rental.Units).Except(occupiedUnits).First();
        }

        public List<CalendarDay> GetCalendarDates(Rental rental, DateTime startDate, int nights)
        {
            List<Booking> bookings = _bookings.GetByRentalId(rental.Id);
            List<CalendarDay> dates = new List<CalendarDay>();

            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDay
                {
                    Date = startDate.Date.AddDays(i),
                    Bookings = new List<Booking>(),
                    PreparationTimes = new List<Preparation>()
                };

                foreach (var booking in bookings)
                {
                    DateTime bookingEndDate = booking.Start.AddDays(booking.Nights);
                    if (booking.Start <= date.Date)
                    {
                        if (bookingEndDate > date.Date)
                        {
                            date.Bookings.Add(new Booking
                            {
                                Id = booking.Id,
                                Unit = booking.Unit
                            });
                        }
                        else if (bookingEndDate.AddDays(rental.PreparationTimeInDays) > date.Date)
                        {
                            date.PreparationTimes.Add(new Preparation
                            {
                                Unit = booking.Unit
                            });
                        }
                    }
                }

                dates.Add(date);
            }

            return dates;
        }

        public bool CheckCapacityForUnitDecrease(Rental rental, int desiredUnitNumber)
        {
            List<Booking> bookings = _bookings.GetByRentalId(rental.Id);

            if (bookings.Any())
            {
                DateTime minBookingStartDate = bookings.Min(x => x.Start.Date);

                DateTime maxBookingEndDate = bookings.Max(x => x.Start.Date.AddDays(x.Nights));

                int maxDuration = maxBookingEndDate.Subtract(minBookingStartDate).Days + rental.PreparationTimeInDays;

                var calendar = GetCalendarDates(rental, minBookingStartDate, maxDuration);

                return calendar.Max(x => x.Bookings.Count + x.PreparationTimes.Count) <= desiredUnitNumber;
            }

            else
            {
                return true;
            }
        }

        public bool CheckCapacityForPreparationTimeIncrease(Rental rental, int desiredPreparationTimeInDays)
        {
            Rental newRental = new Rental
            {
                Id = rental.Id,
                Bookings = rental.Bookings,
                PreparationTimeInDays = desiredPreparationTimeInDays,
                Units = rental.Units
            };

            foreach (var booking in rental.Bookings)
            {
                newRental.Bookings = newRental.Bookings.Except(new[] { booking }, new BookingComparer()).ToList();
                try
                {
                    CheckAvailability(newRental, booking.Start.Date, booking.Nights);
                }
                catch (ApplicationException)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
