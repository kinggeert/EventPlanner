using EventPlanner.Data;
using EventPlanner.Models;
using EventPlanner.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers;

public class AccountController : Controller
{
    private readonly EventDb _context;

    public AccountController(EventDb context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public ActionResult Login(LoginViewModel loginViewModel)
    {
        // Your authentication logic here
        Organiser organiser = AuthenticateOrganiser(loginViewModel.UserName, loginViewModel.Password);

        if (organiser != null)
        {
            // Store the OrganizerID in session
            HttpContext.Session.SetInt32("OrganiserID", organiser.OrganiserId);
            return RedirectToAction("Dashboard", "Events");
        }

        // Handle login failure
        ViewBag.Message = "Invalid login!";
        return View();
    }

    private Organiser AuthenticateOrganiser(string username, string password)
    {
        // Super secure authentication
        var organiser = _context.Organiser.FirstOrDefault(o => o.OrganiserName == username);
        return organiser;
    }
}
