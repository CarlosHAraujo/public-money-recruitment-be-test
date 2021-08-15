using System.Collections.Generic;

namespace VacationRental.Core
{
    public interface IBookingRepository
    {
        List<Booking> GetByRentalId(int rentalId);
        Booking Get(int id);
        int Create(Booking booking);
    }
}
