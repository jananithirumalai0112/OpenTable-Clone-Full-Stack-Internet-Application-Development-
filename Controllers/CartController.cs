using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OpenTableApp.Models;
using OpenTableApp.Data;
using OpenTableApp.ExtensionMethods;
using OpenTableApp.Helpers;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenTableApp.Controllers
{
    

    public class CartController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public CartController(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        //  GET: View cart items
        public IActionResult Cart()
        {

            List<CartItem> cartItems;

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                var items = _dbContext.CartItems
                    .Where(c => c.UserId == userId)
                    .ToList();

                cartItems = _dbContext.CartItems
                                      .Where(c => c.UserId == userId)
                                      .Include(c => c.Restaurant)
                                      .ToList();

                HttpContext.Session.Remove("CartItems");
                HttpContext.Session.Remove("CartCount");

                HttpContext.Session.SetInt32("CartCount", cartItems.Count);
            }
            else
            {
                var cart = new Cart(HttpContext);
                cart.LoadFromCookieIfNeeded();
                cartItems = cart.List.ToList();
            }

            ViewBag.CartCount = cartItems.Count;
            return View(cartItems);
        }

        //  POST: Add item to cart (fully working)
        [HttpPost]
        public IActionResult AddToCart(CartItem cartItem)
        {
            List<CartItem> cartItems;
            

            if (User.Identity.IsAuthenticated)
            
            {
                var userId = _userManager.GetUserId(User);
                cartItem.UserId = userId;
                _dbContext.CartItems.Add(cartItem);
                _dbContext.SaveChanges(); // Save the changes to the database
                var cartCount = _dbContext.CartItems.Count(c => c.UserId == userId);
                ViewBag.CartCount = cartCount;
            }
            else
            {
                cartItems = HttpContext.Session.GetObject<List<CartItem>>("CartItems") ?? new List<CartItem>();
                cartItems.Add(cartItem);
                HttpContext.Session.SetObject("CartItems", cartItems);
                ViewBag.CartCount = cartItems.Count;
            }

            return RedirectToAction("Cart");
        }

        //  POST: Remove item from cart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var item = _dbContext.CartItems.FirstOrDefault(c => c.Id == id && c.UserId == user.Id);
                if (item != null)
                {
                    _dbContext.CartItems.Remove(item);
                    await _dbContext.SaveChangesAsync();
                }
            }
            else
            {
                var cart = new Cart(HttpContext);
                cart.Remove(id);
            }

            TempData["Success"] = "Item removed from cart.";
            return RedirectToAction("Cart");
        }

        //  POST: Clear cart
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var userCartItems = _dbContext.CartItems.Where(c => c.UserId == user.Id);
                _dbContext.CartItems.RemoveRange(userCartItems);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var cart = new Cart(HttpContext);
                cart.Clear();
            }

            TempData["Success"] = "Cart cleared.";
            return RedirectToAction("Cart");
        }
    }
}
