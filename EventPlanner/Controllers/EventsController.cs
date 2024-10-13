using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventPlanner.Data;
using EventPlanner.Models;
using EventPlanner.ViewModels;

namespace EventPlanner.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventDb _context;

        public EventsController(EventDb context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events
                .Where(e => e.EventDate >= DateTime.Today)
                .Include(e => e.EventCategory)
                .ToListAsync());
        }
        
        public ActionResult Dashboard()
        {
            // Retrieve OrganizerID from session
            var organiserIdNullable = HttpContext.Session.GetInt32("OrganiserID");
            if (!organiserIdNullable.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            var organiserId = organiserIdNullable.Value;
            
            // Get events for the logged-in organizer
            var events = _context.Events
                .Where(e => e.EventOrganiser.OrganiserId == organiserId)
                .Include(e => e.EventCategory)
                .ToList();

            return View(events); // Pass events to the view
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.PurchasedTickets)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            // Retrieve Organizer from session
            var organiserIdNullable = HttpContext.Session.GetInt32("OrganiserID");
            if (!organiserIdNullable.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            var organiserId = organiserIdNullable.Value;
            Organiser organiser = _context.Organiser.Find(organiserId);
            if (organiser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var categories = _context.Category.ToList();
            
            EventOrganiserViewModel eventOrganiserViewModel = new EventOrganiserViewModel()
            {
                Organiser = organiser,
                Categories = categories,
                Event = new Event
                {
                    EventCategory = new Category(),
                    EventOrganiser = new Organiser()
                }
            };
            
            return View(eventOrganiserViewModel);
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventOrganiserViewModel viewModel)
        {
            viewModel.Event.EventCategory = await _context.Category.FindAsync(viewModel.SelectedCategoryId);
            if (ModelState.IsValid)
            {
                viewModel.Event.EventOrganiser =
                    await _context.Organiser.FindAsync(viewModel.Organiser.OrganiserId);
                _context.Add(viewModel.Event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            
            // Retrieve Organizer from session
            var organiserIdNullable = HttpContext.Session.GetInt32("OrganiserID");
            if (!organiserIdNullable.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            var organiserId = organiserIdNullable.Value;
            Organiser organiser = _context.Organiser.Find(organiserId);
            if (organiser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var categories = _context.Category.ToList();
            
            EventOrganiserViewModel eventOrganiserViewModel = new EventOrganiserViewModel()
            {
                Organiser = organiser,
                Categories = categories,
                Event = @event
            };
            
            return View(eventOrganiserViewModel);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventOrganiserViewModel viewModel)
        {
            if (id != viewModel.Event.EventId)
            {
                return NotFound();
            }

            viewModel.Event.EventCategory = await _context.Category.FindAsync(viewModel.SelectedCategoryId);
            if (ModelState.IsValid)
            {
                try
                {
                    viewModel.Event.EventOrganiser =
                        await _context.Organiser.FindAsync(viewModel.Organiser.OrganiserId);
                    _context.Add(viewModel.Event);
                    _context.Update(viewModel.Event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(viewModel.Event.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Purchase(int? id)
        {
            if (id == null) return NotFound();
            
            var @event = _context.Events.Find(id);

            if (@event == null) return NotFound();

            var eventTicketViewModel = new EventTicketViewModel()
            {
                Event = @event,
                Ticket = new Ticket
                {
                CustomerBillingAddress = new Address(),
                Event = @event
                }
            };
            return View("Purchase", eventTicketViewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(EventTicketViewModel eventTicketViewModel)
        {
            if (ModelState.IsValid)
            {

                eventTicketViewModel.Ticket.Event = _context.Events.Find(eventTicketViewModel.Event.EventId);
                eventTicketViewModel.Ticket.PurchaseDate = DateTime.Now;
                
                // Add the ticket to the database
                _context.Add(eventTicketViewModel.Ticket);
                await _context.SaveChangesAsync();

                // Redirect to a success page
                return RedirectToAction("PurchaseSuccess");
            }
            
            // If model state is invalid, return the same view with validation errors
            return View(eventTicketViewModel);
        }

        public IActionResult PurchaseSuccess()
        {
            return View();
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
