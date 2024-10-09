namespace EventPlanner.Models;

public class Ticket
{
    public int TicketId { get; set; }
    public Event Event { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public Address CustomerBillingAddress { get; set; }
}