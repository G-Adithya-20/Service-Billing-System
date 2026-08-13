using Microsoft.AspNetCore.Mvc;
using ServiceBillingSystem.Data;
using ServiceBillingSystem.Models;

namespace ServiceBillingSystem.Controllers;

public class CustomersController : Controller
{
    private readonly AppDbContext _context;

    public CustomersController(AppDbContext context)
    {
        _context = context;
    }

    //opens form
    [HttpGet]
    public IActionResult Create()
    {
        var role = HttpContext.Session.GetString("Role");

        if (role != "Staff")
        {
            return RedirectToAction("Login","Users" );
        }

        return View();
    }

    //runs when user submits the customer details form
    [HttpPost]
    public IActionResult Create(Customer customer)
    {
        var role = HttpContext.Session.GetString("Role");

        if (role != "Staff")
        {
            return RedirectToAction("Login","Users");
        }


        if (!ModelState.IsValid)
        {
            return View(customer);
        }
        _context.Customers.Add(customer); //mapping
        _context.SaveChanges();

        TempData["Success"] = "Customer created successfully.";

        return RedirectToAction("Create");
    }
}