using BDJ.Data.Models;
using System.Collections.Generic;
using System;
using BDJ.Services.Contracts;
using System.Linq;
using BDJ.Data;

namespace BDJ.Services
{
    public class ReservationService : IReservationService
    {
        private readonly List<Reservation> _reservations; // Идеално е тук да имате връзка с БД
        private readonly BDJContext _dbContext;

        public ReservationService(BDJContext dbContext)
        {
            _reservations = new List<Reservation>();
            _dbContext = dbContext;
        }

        public Reservation CreateReservation(Guid lineId, string firstName, string middleName, string lastName, string phoneNumber, string userId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var line = _dbContext.Lines.FirstOrDefault(l => l.Id == lineId); // Вземете линията по ID
            if (line == null)
            {
                throw new Exception("Line not found.");
            }

            var reservation = new Reservation
            {
                Line = line,  // Свързване на резервацията с линията
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                User = user,
            };

            _dbContext.Reservations.Add(reservation);  // Добавяне на резервацията в базата данни
            _dbContext.SaveChanges();  // Записване на промените

            return reservation;  // Връщане на създадената резервация
        }


        public List<Reservation> GetReservationsByUser(string userId)
        {
            var reservations = _reservations.Where(r => r.User.Id == userId).ToList();
            Console.WriteLine($"Found {reservations.Count} reservations for user {userId}");
            return reservations;
        }

        public Reservation GetReservationById(Guid reservationId)
        {
            return _reservations.FirstOrDefault(r => r.Id == reservationId);
        }

        public void CancelReservation(Guid reservationId)
        {
            var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation != null)
            {
                _reservations.Remove(reservation);
            }
        }
    }
}
