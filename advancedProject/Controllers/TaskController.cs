using Microsoft.AspNetCore.Mvc;
using advancedProject.Models;
using advancedProject.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Build.Framework;
namespace advancedProject.Controllers
{
    public class TaskController : Controller
    {
        private readonly DBContext dbContext;
        private readonly UserManager<User> userManager; // Adjust the generic type if you are using a custom user type

        public TaskController(DBContext dbContext, UserManager<User> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }


        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var users = await userManager.GetUsersInRoleAsync("user");
            ViewBag.Users = new SelectList(users, "Id", "UserName");
            return View();
        }

        [HttpPost]

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Add(Tasks viewModel)
        {
            var t = new Tasks
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                Date = viewModel.Date,
                Priority = viewModel.Priority,
                UserId = viewModel.UserId,// Assume UserId is coming from form data
                status = viewModel.status, // Set the default status to "Not Started"

            };
            await dbContext.Tasks.AddAsync(t);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("List"); // Redirect to list view after adding the task
        }

        [Authorize(Roles = "admin")]

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var tasks = await dbContext.Tasks.Include(t => t.User).ToListAsync(); // Include the user information
            return View(tasks);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int Id){
            var task = await dbContext.Tasks.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == Id);
            if (task == null){
                return NotFound();
            }
            ViewBag.status = Enum.GetValues(typeof(Status))
                         .Cast<Status>()
                         .Select(s => new SelectListItem
                         {
                             Text = s.ToString(),
                             Value = ((int)s).ToString()
                         });
            if (User.IsInRole("admin")){
                ViewBag.Status = new List<string> { Status.InProgress.ToString(), Status.Completed.ToString() };
            }
            else if (User.IsInRole("user")){
                ViewBag.Status = new List<string> { Status.InProgress.ToString(), Status.Completed.ToString() };
            }
            var users = await userManager.GetUsersInRoleAsync("user");
            ViewBag.Users = new SelectList(users, "Id", "UserName", task.UserId);
            return View(task);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Tasks viewModel, string userId) {
            var tasks = await dbContext.Tasks.FindAsync(viewModel.Id);
            if(tasks is not null){
                tasks.Title= viewModel.Title;
                tasks.Description= viewModel.Description;
                tasks.Date= viewModel.Date;
                tasks.Priority= viewModel.Priority;
                tasks.UserId = userId;
                tasks.status = viewModel.status;
                await dbContext.SaveChangesAsync();
            }
            if (User.IsInRole("admin")){
                // Admins can set any status; setting default status if not already set.
                tasks.status = viewModel.status;
            }
            else if (User.IsInRole("user")){
                // Users can only update the status to either InProgress or Completed
                if (viewModel.status == Status.InProgress || viewModel.status == Status.Completed){
                    tasks.status = viewModel.status;
                }
                else
                {
                    // Optionally, add error handling or logging here if an invalid status is attempted
                    ModelState.AddModelError("Status", "You are not allowed to set this status.");
                    return View(viewModel);  // Return to the view with an error message.
                }
            }
                return RedirectToAction("List", "Task");
        }

        [Authorize(Roles = "admin")]

        [HttpPost]
        public async Task<IActionResult> Delete(Tasks viewModel)
        {
            var task = await dbContext.Tasks.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == viewModel.Id);

            if (task != null)
            {
                dbContext.Tasks.Remove(task);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Task");
        }


        [Authorize(Roles="user")]
        [HttpGet]
        public async Task<IActionResult> TasksByUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var tasks = await dbContext.Tasks
                        .Where(t => t.UserId == userId)
                        .ToListAsync();

            ViewData["UserName"] = user.UserName; // Pass the user's name to the view for display
            return View("UserTasks", tasks);
        }




        [HttpGet]
        public async Task<IActionResult> UserEditTask(int id)
        {
            var task = await dbContext.Tasks.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Status)), task.status);
            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> UserTasks(Tasks viewModel)
        {
            var tasks = await dbContext.Tasks.FindAsync(viewModel.Id);

            if (tasks is not null)
            {
               
                tasks.status = viewModel.status;

                await dbContext.SaveChangesAsync();

            }


                return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            return View();
        }


    }
}
