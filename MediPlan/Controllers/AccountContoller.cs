using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MediPlan.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Na primer, profil stranica korisnika
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Ako želiš da dobaviš dodatne podatke (npr. claimove, uloge, itd.)
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            return View(user);
        }

        // Logout logika
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
