using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceBillingSystem.Data;
using ServiceBillingSystem.Models;

namespace ServiceBillingSystem.Controllers;

public class UsersController : Controller
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string email,string password)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == email);

        if (user == null)
        {
            ViewBag.Error = "Invalid email or password";
            return View();
        }
        // Check whether the account is active
        if (!user.IsActive)
        {
            ViewBag.Error = "This account is no longer active.";
            return View();
        }

        var passwordHasher = new PasswordHasher<User>();

        var result = passwordHasher.VerifyHashedPassword(user,user.Password,password);

        if (result == PasswordVerificationResult.Failed)
        {
            ViewBag.Error = "Invalid email or password";
            return View();
        }

        // Store login information in Session
        HttpContext.Session.SetInt32("UserId",user.Id);

        HttpContext.Session.SetString("UserName",user.Name);

        HttpContext.Session.SetString("Role",user.Role);

        // Redirect according to role
        if (user.Role == "Admin")
        {
            return RedirectToAction("AdminDashboard","Users");
        }

        return RedirectToAction("StaffDashboard","Users");
    }

    [HttpGet]
    public IActionResult AdminDashboard()
    {
        // Check whether logged-in user is Admin

        var role = HttpContext.Session.GetString("Role");

        if (role != "Admin")
        {
            return RedirectToAction("Login");
        }


        var model = new AdminDashboardViewModel
        {
            TotalStaff =_context.Users.Count(x => x.Role == "Staff"),

            TotalCustomers = _context.Customers.Count(),

            TotalServices =_context.Services.Count(),

            TotalBills = _context.Bills.Count()
        };


        return View(model);
    }

    [HttpGet]
    public IActionResult Staff()
    {
        var role = HttpContext.Session.GetString("Role");

        if (role != "Admin")
        {
            return RedirectToAction("Login");
        }


        var staff =_context.Users.Where(x => x.Role == "Staff").ToList();


        return View(staff);
    }

    [HttpGet]
    public IActionResult AddStaff()
    {
        var role = HttpContext.Session.GetString("Role");

        if (role != "Admin")
        {
            return RedirectToAction("Login");
        }

        return View();
    }

    [HttpPost]
    public IActionResult AddStaff(User user)
    {
        var role = HttpContext.Session.GetString("Role");

        if (role != "Admin")
        {
            return RedirectToAction("Login");
        }


        if (!ModelState.IsValid)
        {
            return View(user);
        }

       var existingUser = _context.Users.FirstOrDefault(x => x.Email == user.Email);

        if (existingUser != null)
        {
            ModelState.AddModelError( "Email","Email already exists");
            return View(user);
        }

        // Public signup can create Staff only.
        user.Role = "Staff";

        var passwordHasher = new PasswordHasher<User>();

        user.Password =passwordHasher.HashPassword(user, user.Password);

        _context.Users.Add(user);

        _context.SaveChanges();

        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult StaffDashboard()
    {
        var role = HttpContext.Session.GetString("Role");

        if (role != "Staff")
        {
            return RedirectToAction("Login");
        }

        var staffId = HttpContext.Session.GetInt32("UserId");

        if (staffId == null)
        {
            return RedirectToAction("Login");
        }

        ViewBag.TotalCustomers = _context.Customers.Count();

        // Only bills created by the logged-in staff
        ViewBag.TotalBills = _context.Bills.Count(x => x.StaffId == staffId.Value);

        return View();
    }

    [HttpGet]
    public IActionResult AddAdmin()
    {
        var role = HttpContext.Session.GetString("Role");

        if (role != "Admin")
        {
            return RedirectToAction("Login");
        }

        return View();
    }

    [HttpPost]
    public IActionResult AddAdmin(User user)
    {
        var role = HttpContext.Session.GetString("Role");

        if (role != "Admin")
        {
            return RedirectToAction("Login");
        }

        if (!ModelState.IsValid)
        {
            return View(user);
        }

        var existingUser = _context.Users
            .FirstOrDefault(x => x.Email == user.Email);

        if (existingUser != null)
        {
            ModelState.AddModelError(
                "Email",
                "Email already exists"
            );

            return View(user);
        }

        // Create new permanent Admin
        user.Role = "Admin";
        user.IsInitialAdmin = false;
        user.IsActive = true;

        var passwordHasher = new PasswordHasher<User>();

        user.Password =
            passwordHasher.HashPassword(user, user.Password);

        // Save the new Admin first
        _context.Users.Add(user);
      
        // New Admin was successfully created.
        // Now disable the one-time hardcoded Admin.

        var initialAdmin = _context.Users
            .FirstOrDefault(x => x.IsInitialAdmin);

        if (initialAdmin != null)
        {
            // Soft delete the initial Admin
            initialAdmin.IsActive = false;
        }
            _context.SaveChanges();
        // Logout the hardcoded Admin
        HttpContext.Session.Clear();


        return RedirectToAction("AdminDashboard");
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login");
    }
}