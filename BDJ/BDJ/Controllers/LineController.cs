using BDJ.Data.Models;
using BDJ.Models;
using BDJ.Services;
using BDJ.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace BDJ.Controllers
{
    public class LineController : Controller
    {
        private readonly ILine lineService;
        private readonly ITrain trainService;
        private readonly IReservationService _reservationService;
        private readonly ILogger<LineController> _logger;

        public LineController(ILine lineService, ITrain trainService, IReservationService reservationService, ILogger<LineController> logger)
        {
            this.lineService = lineService;
            this.trainService = trainService;
            this._reservationService = reservationService;
            this._logger = logger;
        }

        public IActionResult Index()
        {
            LinesIndexViewModel model = new LinesIndexViewModel()
            {
                Lines = lineService.GetAll().Select(l => new LineIndexViewModel()
                {
                    ArrivalTime = l.ArrivalTime.ToString("hh:mm"),
                    Date = l.Date.ToString("MM/dd/yyyy"),
                    Departure = l.Departure,
                    DepartureTime = l.DepartureTime.ToString("hh:mm"),
                    Destination = l.Destination,
                    TrainNumber = trainService.GetById(l.TrainId).Number,
                    LineId = l.Id
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            LineCreateViewModel model = new LineCreateViewModel()
            {
                Trains = trainService.GetAll(),
                DepartureTime = DateTime.Now.Date,
                ArrivalTime = DateTime.Now.Date,
                Date = DateTime.Now.Date
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(LineCreateViewModel model)
        {
            Line line = new Line()
            {
                ArrivalTime = model.ArrivalTime,
                Departure = model.Departure,
                DepartureTime = model.DepartureTime,
                Destination = model.Destination,
                Train = trainService.GetById(model.TrainId),
            };
            lineService.Create(line);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Guid id)
        {
            Line line = lineService.GetById(id);
            if (line == null)
            {
                _logger.LogError("Line with id {Id} not found.", id);
                return NotFound();
            }

            var model = new LineEditViewModel()
            {
                ArrivalTime = line.ArrivalTime,
                Date = line.Date,
                Departure = line.Departure,
                DepartureTime = line.DepartureTime,
                Destination = line.Destination,
                TrainId = line.TrainId,
                Trains = trainService.GetAll(),
                LineId = line.Id
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(LineEditViewModel model)
        {
            Line line = new Line()
            {
                ArrivalTime = model.ArrivalTime,
                Date = model.Date,
                Departure = model.Departure,
                DepartureTime = model.DepartureTime,
                Destination = model.Destination,
                Id = model.LineId,
                TrainId = model.TrainId,
                Train = trainService.GetById(model.TrainId)
            };

            lineService.Update(line);

            return RedirectToAction("Index");
        }

        public IActionResult Search(string? searchString)
        {
            LineSearchViewModel model;
            if (searchString != null)
            {
                model = new LineSearchViewModel()
                {
                    Lines = lineService.GetAll().Where(l => l.Destination.Contains(searchString)
                        || l.Departure.Contains(searchString)
                        || l.DepartureTime.ToString().Contains(searchString)).Select(l => new Line()
                        {
                            ArrivalTime = l.ArrivalTime,
                            Date = l.Date,
                            Departure = l.Departure,
                            DepartureTime = l.DepartureTime,
                            Destination = l.Destination,
                            Train = trainService.GetById(l.TrainId),
                            Id = l.Id,
                            TrainId = l.TrainId
                        }).ToList()
                };
            }
            else
            {
                model = new LineSearchViewModel()
                {
                    Lines = lineService.GetAll().Select(l => new Line()
                    {
                        ArrivalTime = l.ArrivalTime,
                        Date = l.Date,
                        Departure = l.Departure,
                        DepartureTime = l.DepartureTime,
                        Destination = l.Destination,
                        Train = trainService.GetById(l.TrainId),
                        Id = l.Id,
                        TrainId = l.TrainId
                    }).ToList()
                };
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Reserve(Guid id)
        {
            var line = lineService.GetById(id);  // Вземете линията по ID
            if (line == null)
            {
                _logger.LogError("Line with id {Id} not found.", id);  // Логнете грешката за несъществуваща линия
                return NotFound();  // Ако линията не съществува, връщаме 404
            }

            var model = new ReservationCreateViewModel
            {
                LineId = line.Id,
                TrainNumber = line.Train?.Number ?? "Unknown",  // Проверете дали Train е null, и поставете "Unknown" ако е null
                DepartureTime = line.DepartureTime,
                ArrivalTime = line.ArrivalTime
            };

            return View(model);  // Извикваме изгледа за резервация
        }

        [HttpPost]
        public IActionResult Reserve(ReservationCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Вземи ID на потребителя
                var reservation = _reservationService.CreateReservation(
                    model.LineId,
                    model.FirstName,
                    model.MiddleName,
                    model.LastName,
                    model.PhoneNumber,
                    userId // Предайте userId, за да свържете резервацията с потребителя
                );

                // Пренасочваме към "My tickets", който показва резервациите на текущия потребител
                return RedirectToAction("Index", "Reservation");
            }

            return View(model);  // Връщаме обратно формата при грешка
        }

    }
}
