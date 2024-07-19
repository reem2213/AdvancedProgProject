using advancedProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using advancedProject.Models;

namespace advancedProject.Areas.Identity.Data;

public class DBContext : IdentityDbContext<User>
{
    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }
    public DbSet<Tasks> Tasks { get; set; }


    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tasks>()
            .HasOne(t => t.User)  // Specifies that each Task has one User
            .WithMany()  // Optionally specify if User has a collection of Tasks
            .HasForeignKey(t => t.UserId);  // Link foreign key to UserId property
    }


}
