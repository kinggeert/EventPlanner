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
        modelBuilder.Entity<Event>(Entity =>
        {
            Entity.Property(e => e.EventName).HasMaxLength(30);
            Entity.HasOne(e => e.EventCategory)
                .WithMany(e => e.Events);
        });

        modelBuilder.Entity<Category>(Entity =>
        {
            Entity.HasMany(e => e.Events)
                .WithOne(e => e.EventCategory);
        });

        modelBuilder.Entity<Ticket>(Entity =>
        {
            Entity.HasOne(e => e.Event)
                .WithMany(e => e.PurchasedTickets);
            Entity.HasOne(e => e.CustomerBillingAddress);
        });

        modelBuilder.Entity<Organiser>(Entity =>
        {
            Entity.HasMany(e => e.OrganisedEvents)
                .WithOne(e => e.EventOrganiser);
        });

    }

public DbSet<EventPlanner.Models.Category> Category { get; set; } = default!;
}