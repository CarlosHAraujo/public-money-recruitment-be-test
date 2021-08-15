using System.Collections.Generic;

namespace VacationRental.Core
{
    public class Rental
    {
        public int Id { get; set; }
        public int Units { get; set; }
        public int PreparationTimeInDays { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
