using BDJ.Data.Models;
using System.Collections.Generic;
using System;

namespace BDJ.Services.Contracts
{
    public interface IReservationService
    {
        Reservation CreateReservation(Guid lineId, string firstName, string middleName, string lastName, string phoneNumber, string userId);

        // Получаване на резервации по потребителски идентификатор
        List<Reservation> GetReservationsByUser(string userId);

        // Получаване на резервация по идентификатор
        Reservation GetReservationById(Guid reservationId);

        // Отказване на резервация
        void CancelReservation(Guid reservationId);
    }
}
