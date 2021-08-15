using System;
using System.Collections.Generic;

namespace VacationRental.Core
{
    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<Preparation> PreparationTimes { get; set; }
    }
}
