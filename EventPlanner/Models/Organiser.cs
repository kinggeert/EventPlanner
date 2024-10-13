namespace EventPlanner.Models;

public class Organiser
{
    public int OrganiserId { get; set; }
    public string? OrganiserName { get; set; }
    public string? OrganiserEmail { get; set; }
    public ICollection<Event>? OrganisedEvents { get; set; }
}