using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTableApp.Data;
using OpenTableApp.Models;
using OpenTableApp.Helpers;
using System;
using Microsoft.AspNetCore.Identity;


using OpenTableApp.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;

namespace OpenTable.Controllers
{
    public class HoldTableController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public HoldTableController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult HoldTable(int RestaurantId, string SelectedTime, DateTime ReservationDate, int NumberOfPeople)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var cartItems = HttpContext.Session.GetObject<List<CartItem>>("CartItems") ?? new List<CartItem>();

                var restaurant = _context.Restaurants
                    .Include(r => r.Metropolis)
                    .FirstOrDefault(r => r.Id == RestaurantId);

                if (restaurant == null)
                {
                    return NotFound();
                }

                var alreadyHeld = cartItems.Any(c =>
                    c.RestaurantId == RestaurantId &&
                    c.Date == ReservationDate &&
                    c.Time == SelectedTime);

                if (alreadyHeld)
                {
                    TempData["Error"] = "You have already held a reservation for this time slot.";
                    return RedirectToAction("Cart", "Cart");
                }

                var cartItem = new CartItem
                {
                    RestaurantId = RestaurantId,
                    RestaurantName = restaurant.Name,
                    MetropolisName = restaurant.Metropolis?.Name,
                    Time = SelectedTime,
                    Date = ReservationDate,
                    NumberOfPeople = NumberOfPeople
                };

                cartItems.Add(cartItem);
                HttpContext.Session.SetObject("CartItems", cartItems);

                TempData["Success"] = "Table is successfully added to Cart. Check Below...";
                ViewBag.CartCount = cartItems.Count;

                return RedirectToAction("Cart", "Cart");
            }
            else
            {
                var userId = _userManager.GetUserId(User);

                var restaurant = _context.Restaurants
                    .Include(r => r.Metropolis)
                    .FirstOrDefault(r => r.Id == RestaurantId);

                if (restaurant == null)
                {
                    return NotFound();
                }

                var alreadyHeld = _context.CartItems.Any(c =>
                    c.UserId == userId &&
                    c.RestaurantId == RestaurantId &&
                    c.Date == ReservationDate &&
                    c.Time == SelectedTime);

                if (alreadyHeld)
                {
                    TempData["Error"] = "You have already held a reservation for this time slot.";
                    return RedirectToAction("Cart", "Cart");
                }

                var cartItem = new CartItem
                {
                    RestaurantId = RestaurantId,
                    RestaurantName = restaurant.Name,
                    MetropolisName = restaurant.Metropolis?.Name,
                    Time = SelectedTime,
                    Date = ReservationDate,
                    NumberOfPeople = NumberOfPeople,
                    UserId = userId
                };

                _context.CartItems.Add(cartItem);
                _context.SaveChanges();

                TempData["Success"] = "Table is successfully added to Cart. Check Below...";
                ViewBag.CartCount = _context.CartItems.Count(c => c.UserId == userId);

                return RedirectToAction("Cart", "Cart");
            }
        }
    }
}
