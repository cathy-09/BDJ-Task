using System;

namespace BDJ.Models
{
    public class ReservationCreateViewModel
    {
        public Guid LineId { get; set; }  // Идентификатор на линията, която се резервира
        public string FirstName { get; set; }  // Име на потребителя
        public string MiddleName { get; set; }
        public string LastName { get; set; }  // Фамилия на потребителя
        public string PhoneNumber { get; set; }  // Телефонен номер на потребителя

        // Допълнителни полета, ако искате да ги показвате в изгледа за резервация
        public string TrainNumber { get; set; }  // Номер на влака
        public DateTime DepartureTime { get; set; }  // Време на заминаване
        public DateTime ArrivalTime { get; set; }  // Време на пристигане
    }
}
