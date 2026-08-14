using Microsoft.AspNetCore.Mvc;
using ServiceBillingSystem.Data;
using ServiceBillingSystem.Models;

namespace ServiceBillingSystem.Controllers;

public class ServicesController : Controller
{
    private readonly AppDbContext _context;

    public ServicesController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var services = _context.Services.ToList();

        return View(services);
    }

    public IActionResult Create() //open create service form
    {
        return View();
    }

    [HttpPost] //runs when user submits create form
    public IActionResult Create(Service service)
    {
        if (!ModelState.IsValid)
        {
            return View(service);
        }

        _context.Services.Add(service);

        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpGet] //service autocomplete search for bill creation
    public IActionResult Search(string term) //Services/Search?term=
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return Json(new List<object>());
        }

        var services = _context.Services
            .Where(x => x.IsActive && x.Name.Contains(term))
            .Select(x => new
            {
                id = x.Id,
                name = x.Name,
                price = x.Price
            })
            .Take(10)
            .ToList(); //max 10 matching services

        return Json(services); //sends result back to browser(fetch) as JSON
    }
}