namespace EventPlanner.Models;

public class Event
{
    public int EventId { get; set; }
    public string EventName { get; set; }
    public string EventDescription { get; set; }
    public DateTime EventDate { get; set; }
    public double TicketPrice { get; set; }
    public Category EventCategory { get; set; }
    public Organiser EventOrganiser { get; set; }
    public ICollection<Ticket> PurchasedTickets { get; set; }
}