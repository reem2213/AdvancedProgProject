using advancedProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace advancedProject.Models
{
    public class Tasks
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public int Priority { get; set; }

        public Status status { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }


    }
    public enum Status
    {
        NotStarted,
        InProgress,
        Completed,
        OnHold
    }

}
