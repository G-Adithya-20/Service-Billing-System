using Microsoft.AspNetCore.Mvc;
using ServiceBillingSystem.Data;
using ServiceBillingSystem.Models;

namespace ServiceBillingSystem.Controllers
{
    public class CompanyController : Controller
    {
        private readonly AppDbContext _context;

        public CompanyController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var company = _context.Companies.FirstOrDefault(); //get the first company record(cam have only 1 company details)

            if (company == null)
            {
                company = new Company(); //allows form to open with empty fields if no company record exists
            }

            return View(company);
        }

        //runs when user submits the company details form
        [HttpPost]
        public IActionResult Save(Company model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var company = _context.Companies.FirstOrDefault();

            if (company == null) //no company exists,create a new one
            {
                _context.Companies.Add(model);
            }
            else
            {
                company.name = model.name;
                company.address = model.address;
                company.phonenumber = model.phonenumber;
                company.email = model.email;
            }

            _context.SaveChanges();
            TempData["Success"] = "Company details updated successfully.";

            return RedirectToAction("Index");
        }
    }
}