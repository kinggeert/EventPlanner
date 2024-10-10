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
    public int AvailableTickets { get; set; }

    /// <summary>
    /// Returns the amount of tickets remaining for the event.
    /// </summary>
    /// <returns></returns>
    public int GetRemainingTickets()
    {
        int Remaning = AvailableTickets - PurchasedTickets.Count;
        return Remaning;
    }
}