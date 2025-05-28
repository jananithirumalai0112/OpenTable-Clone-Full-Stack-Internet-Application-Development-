using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTableApp.Data;
using OpenTableApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenTable.Controllers
{
    public class DetailsController : Controller
    {
        private readonly AppDbContext _context;

        public DetailsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Detail(int id, DateTime? selectedDate = null)
        {
            var restaurant = _context.Restaurants
                .Include(r => r.Metropolis)
                .FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
            {
                return View("Error", new ErrorViewModel { Message = "Restaurant not found." });
            }

            // Normalize dashes and split open hours
            var openHourParts = restaurant?.OpenHours?
                .Replace("–", "-")
                .Split('-');

            if (openHourParts == null || openHourParts.Length < 2)
            {
                return View("Error", new ErrorViewModel
                {
                    Message = "Open hours format is invalid. Expected format like '11:00 AM - 10:00 PM'."
                });
            }

            // List of supported time formats
            string[] timeFormats = new[]
            {
                "h:mm tt", "hh:mm tt", "h:mmtt", "hh:mmtt", "HH:mm", "H:mm", "h tt", "htt"
            };

            // Try parsing open time
            if (!DateTime.TryParseExact(openHourParts[0].Trim(), timeFormats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var openDateTime))
            {
                return View("Error", new ErrorViewModel
                {
                    Message = $"Could not parse opening time: '{openHourParts[0].Trim()}'"
                });
            }

            // Try parsing close time
            if (!DateTime.TryParseExact(openHourParts[1].Trim(), timeFormats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var closeDateTime))
            {
                return View("Error", new ErrorViewModel
                {
                    Message = $"Could not parse closing time: '{openHourParts[1].Trim()}'"
                });
            }

            var openTime = openDateTime.TimeOfDay;
            var closeTime = closeDateTime.TimeOfDay;

            if (closeTime <= openTime)
            {
                closeTime = closeTime.Add(TimeSpan.FromDays(1)); // Handle overnight timing
            }

            // Generate full-hour time slots only
            var allSlots = new List<string>();
            var firstHour = TimeSpan.FromHours(Math.Ceiling(openTime.TotalHours));
            var lastHour = TimeSpan.FromHours(Math.Floor(closeTime.TotalHours));

            for (var time = firstHour; time <= lastHour; time = time.Add(TimeSpan.FromHours(1)))
            {
                allSlots.Add(DateTime.Today.Add(time).ToString("h tt")); // e.g., 12 PM, 1 PM, 2 PM
            }

            var dateToCheck = selectedDate?.Date ?? DateTime.Today;

            var reservedTimes = _context.Reservations
                .Where(r => r.RestaurantId == id && r.ReservationDate.Date == dateToCheck)
                .Select(r => r.ReservationTime!)
                .ToList();

            var heldTimes = _context.CartItems
                .Where(c => c.RestaurantId == id && c.Date.Date == dateToCheck)
                .Select(c => c.Time)
                .ToList();

            var unavailableTimes = reservedTimes.Concat(heldTimes).Distinct().ToList();

            var availableSlots = allSlots
                .Where(slot =>
                    !(dateToCheck.Date == DateTime.Today &&
                      DateTime.Parse(slot) < DateTime.Now))
                .Except(unavailableTimes)
                .ToList();

            var viewModel = new ReservationDetailViewModel
            {
                Restaurant = restaurant,
                AvailableTimeSlots = availableSlots,
                ReservationDate = dateToCheck
            };

            return View(viewModel);
        }
    }
}
