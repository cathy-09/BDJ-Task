using System;

namespace BDJ.Models
{
    public class ReservationViewModel
    {
        public Guid Id { get; set; }
        public Guid LineId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        // Информация за линията (по избор)
        public string Departure { get; set; }
        public string Number { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
