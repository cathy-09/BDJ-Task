using BDJ.Data;
using BDJ.Data.Models;
using BDJ.Models;
using BDJ.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;

namespace BDJ.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly ILine _lineService;
        private readonly BDJContext _dbContext;

        public ReservationController(IReservationService reservationService, ILine lineService, BDJContext dbContext)
        {
            _reservationService = reservationService;
            _lineService = lineService;
            _dbContext= dbContext;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // или User.Identity.Name
            var reservations = _dbContext.Reservations
                .Include(r => r.Line)
                .ThenInclude(l => l.Train)
                .Where(r => r.User.Id == userId)
                .Select(r => new ReservationViewModel
                {
                    Id = r.Id,
                    LineId = r.Line.Id,
                    Number = r.Line.Train.Number,  // Добавяме номера на влака тук
                    Departure = r.Line.Departure,
                    Destination = r.Line.Destination,
                    DepartureTime = r.Line.DepartureTime,
                    ArrivalTime = r.Line.ArrivalTime,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    PhoneNumber = r.PhoneNumber
                }).ToList();

            return View(reservations);
        }
        [HttpPost]
        [Authorize] // За да е достъпно само за влезли потребители
        public IActionResult Cancel(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Вземи ID на текущия потребител
            var reservation = _dbContext.Reservations
                .Include(r => r.Line)
                .FirstOrDefault(r => r.Id == id && r.User.Id == userId); // Потвърдете, че резервацията принадлежи на текущия потребител

            if (reservation == null)
            {
                return NotFound(); // Ако не е намерена резервацията
            }

            _dbContext.Reservations.Remove(reservation); // Премахваме резервацията
            _dbContext.SaveChanges(); // Записваме промените в базата данни

            return RedirectToAction("Index"); // Пренасочваме към списъка с резервации
        }
    }
}
