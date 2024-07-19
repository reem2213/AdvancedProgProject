using advancedProject.Areas.Identity.Data;
using advancedProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace advancedProject.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly DBContext _dbContext;



        public HomeController(ILogger<HomeController> logger, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, DBContext dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _dbContext = dbContext;
            _roleManager = roleManager;
        }

       

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> Index()
        {
            // Get the "user" role object
            var role = await _roleManager.FindByNameAsync("user");
            if (role == null)
            {
                ViewData["UserCount"] = 0;
            }
            else
            {
                // Get the list of users in the "user" role
                var usersInRole = await _userManager.GetUsersInRoleAsync("user");
                ViewData["UserCount"] = usersInRole.Count;
            }

            var taskCount = await _dbContext.Tasks.CountAsync();
            ViewData["TaskCount"] = taskCount;


            return View();
        }

    }
}
