namespace EventPlanner.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public ICollection<Event>? Events { get; set; }
}