using EventPlanner.Models;

namespace EventPlanner.ViewModels;

public class EventTicketViewModel
{
    public required Event Event { get; set; }
    public Ticket? Ticket { get; set; }
}