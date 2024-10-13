using EventPlanner.Models;

namespace EventPlanner.ViewModels;

public class EventOrganiserViewModel
{
    public required Organiser Organiser { get; set; }
    public ICollection<Category>? Categories { get; set; }
    public int SelectedCategoryId { get; set; }

    public Event? Event { get; set; }
}