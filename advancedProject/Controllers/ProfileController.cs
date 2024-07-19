using advancedProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace advancedProject.Controllers
{
    [Authorize(Roles = "user")]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;

        public ProfileController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

       

        [Route("profile")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); // Gets the current logged-in user
            if (user == null)
            {
                return NotFound("Unable to load user.");
            }

           return View("~/Views/Profile.cshtml", user);
        }


    }
}
