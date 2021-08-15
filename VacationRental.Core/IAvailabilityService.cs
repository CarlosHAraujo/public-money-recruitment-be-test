using System;
using System.Collections.Generic;

namespace VacationRental.Core
{
    public interface IAvailabilityService
    {
        /// <summary>
        /// Check the availability of a booking and return a Unit if available.
        /// </summary>
        /// <param name="rental">The <see cref="Rental"/> where to book a Unit.</param>
        /// <param name="startDate">The booking start date.</param>
        /// <param name="nights">The duration of booking.</param>
        /// <returns>A nondeterministic unit available for the booking requested.</returns>
        int CheckAvailability(Rental rental, DateTime startDate, int nights);
        bool CheckCapacityForPreparationTimeIncrease(Rental rental, int desiredPreparationTimeInDays);
        bool CheckCapacityForUnitDecrease(Rental rental, int desiredUnitNumber);

        /// <summary>
        /// Get Calendar dates for Bookings of a Rental
        /// </summary>
        /// <param name="rental">The <see cref="Rental"/> where the bookings belong.</param>
        /// <param name="startDate">The Start Date of the <see cref="Calendar"/>.</param>
        /// <param name="nights">The span of nights to build the <see cref="Calendar"/>.</param>
        /// <returns>The <see cref="Calendar"/> object with Bookings and Preparation Dates.</returns>
        List<CalendarDay> GetCalendarDates(Rental rental, DateTime startDate, int nights);
    }
}
