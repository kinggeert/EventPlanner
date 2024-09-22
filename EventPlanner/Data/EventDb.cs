using EventPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EventPlanner.Data;

public class EventDb : DbContext
{
    public DbSet<Event> Events { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connection =
            @"Data Source=.\MSSQLSERVER01;Initial Catalog=EventDb;Integrated Security=true;TrustServerCertificate=true;";
        optionsBuilder
            .UseSqlServer(connection)
            .ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning)
            );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>().Property(e => e.EventName).HasMaxLength(30);
        modelBuilder.Entity<Event>().Property(e => e.EventDescription).HasMaxLength(200);

        EventCategory eventCategory = new EventCategory() { CategoryId = 1, CategoryName = "TestCategory" };

        List<EventCategory> eventCategories = new List<EventCategory>();
        eventCategories.Add(eventCategory);
        Event eventA = new Event()
        {
            EventId = 1,
            EventName = "Test Event",
            EventDescription = "Description of test event",
            EventDate = DateTime.Today,
            Categories = eventCategories,
            TicketPrice = 10
        };

        modelBuilder.Entity<EventCategory>().HasData(eventCategory);
        modelBuilder.Entity<Event>().HasData(eventA);
    }
}