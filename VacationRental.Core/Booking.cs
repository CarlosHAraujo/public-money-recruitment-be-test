﻿using System;
using System.Collections.Generic;

namespace VacationRental.Core
{
    public class Booking
    {
        public int Id { get; set; }

        public int RentalId { get; set; }

        private DateTime _startIgnoreTime;
        public DateTime Start
        {
            get => _startIgnoreTime;
            set => _startIgnoreTime = value.Date;
        }

        public int Nights { get; set; }

        public int Unit { get; set; }
    }

    public class BookingComparer : IEqualityComparer<Booking>
    {
        public bool Equals(Booking x, Booking y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Booking obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
