using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTableApp.Data;
using OpenTableApp.Models;
using System.Linq;

namespace OpenTableApp.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReservationController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult ConfirmReservations(string returnUrl = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please register to continue with your reservation.";
                return RedirectToAction("Register", "Account", new { returnUrl });
            }

            var userId = _userManager.GetUserId(User);
            var cart = _context.CartItems.Where(c => c.UserId == userId).ToList();

            foreach (var item in cart)
            {
                var duplicateExists = _context.Reservations.Any(r =>
                    r.UserId == userId &&
                    r.RestaurantId == item.RestaurantId &&
                    r.ReservationDate == item.Date &&
                    r.ReservationTime == item.Time);

                if (duplicateExists)
                    continue;

                var reservation = new Reservation
                {
                    UserId = userId,
                    RestaurantId = item.RestaurantId,
                    ReservationDate = item.Date,
                    ReservationTime = item.Time,
                    NumberOfPeople = item.NumberOfPeople,
                    IsConfirmed = true
                };

                _context.Reservations.Add(reservation);
            }

            _context.CartItems.RemoveRange(cart);
            _context.SaveChanges();

            ViewBag.CartCount = 0;
            TempData["Success"] = "Reservation Confirmed Successfully!";

            return RedirectToAction("ManageReservations");
        }

        public IActionResult ManageReservations()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please Login to continue with your reservation.";
                return RedirectToAction("Login", "Account");
            }

            var userId = _userManager.GetUserId(User);
            var reservations = _context.Reservations
                .Include(r => r.Restaurant)
                .Where(r => r.UserId == userId)
                .ToList();

            return View(reservations);
        }

        [HttpPost]
        public IActionResult CancelReservation(int reservationId)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
            {
                TempData["Error"] = "Reservation not found.";
                return RedirectToAction("ManageReservations");
            }

            var userId = _userManager.GetUserId(User);
            if (reservation.UserId != userId)
            {
                TempData["Error"] = "You are not authorized to cancel this reservation.";
                return RedirectToAction("ManageReservations");
            }

            _context.Reservations.Remove(reservation);
            _context.SaveChanges();

            TempData["Success"] = "Reservation canceled successfully!";
            return RedirectToAction("ManageReservations");
        }
    }
}
