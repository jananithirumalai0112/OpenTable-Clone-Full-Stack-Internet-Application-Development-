using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenTableApp.Models;
using OpenTableApp.Data;
using OpenTableApp.ExtensionMethods;
using OpenTableApp.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace OpenTableApp.Controllers
{
    public class AccountController : Controller 
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _dbContext;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, AppDbContext dbContext)
        {
            _userManager = userManager;  
            _signInManager = signInManager; 
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    DOB = model.DOB,
                    CreatedAt = System.DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // ✅ Migrate guest cart to DB
                    var cart = new Cart(HttpContext); // Get the cart from session
                    var guestCart = cart.List.ToList(); // Convert to List for easier manipulation

                    if (guestCart.Any()) // Check if there are items in the cart
                    {
                        foreach (var item in guestCart) //
                        {
                            item.UserId = user.Id; 
                            item.User = null; 
                            _dbContext.CartItems.Add(item); 
                        }

                        await _dbContext.SaveChangesAsync();
                        cart.Clear();
                    }

                    TempData["SuccessMessage"] = "You are successfully registered. Welcome to Open Table.";
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl ?? Url.Content("~/") });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, model.RememberMe);

                        HttpContext.Session.Remove("CartItems");
                        HttpContext.Session.Remove("CartCount");

                        return LocalRedirect(model.ReturnUrl ?? Url.Content("~/"));
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Optional: Legacy method, not needed if using [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckAdminAccess()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var user =  await _userManager.GetUserAsync(User);
            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }

            return RedirectToAction("AccessDenied");
        }
    }
}
