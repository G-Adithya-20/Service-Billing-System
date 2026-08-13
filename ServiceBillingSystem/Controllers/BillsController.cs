using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using ServiceBillingSystem.Data;
using ServiceBillingSystem.Models;
using ServiceBillingSystem.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServiceBillingSystem.Controllers
{
    public class BillsController : Controller
    {
        private readonly AppDbContext _context;

        public BillsController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Staff")
            {
                return Unauthorized();
            }

            return View();
        }


        [HttpPost] //actual bill creation
        public IActionResult Create([FromBody] BillCreateRequest request)
        {
            
            var role = HttpContext.Session.GetString("Role");

            if (role != "Staff")
            {
                return Unauthorized();
            }

           
            var staffId = HttpContext.Session.GetInt32("UserId");

            if (staffId == null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                return BadRequest("Customer is required");
            }

            var customer = _context.Customers.FirstOrDefault(x => x.Name == request.CustomerName);

            if (customer == null)
            {
                return BadRequest("Customer not found");
            }


            if (request.Items == null || request.Items.Count == 0)
            {
                return BadRequest("At least one service is required");
            }


            decimal subTotal = 0; //total price of all services before discount and tax

            var billItems = new List<BillItem>();


            foreach (var item in request.Items) 
            {
                if (item.Quantity < 1)
                {
                    return BadRequest("Quantity must be at least 1");
                }

                //check - service is active
                var service = _context.Services.FirstOrDefault(x =>x.Id == item.ServiceId && x.IsActive);

                if (service == null)
                {
                    return BadRequest("Service not found");
                }


                decimal total = service.Price * item.Quantity;

                subTotal += total; //bill has multiple services,so we add the total of each service to the subtotal

                //object for each service in the bill
                billItems.Add(new BillItem
                {
                    ServiceId = service.Id,
                    Quantity = item.Quantity,
                    UnitPrice = service.Price,
                    Total = total
                });
            }

            
            decimal discount = request.Discount; //discount entered by staff

            if (discount < 0)
            {
                discount = 0;
            }

            if (discount > subTotal)
            {
                return BadRequest("Discount cannot be greater than subtotal");
            }


            decimal tax = subTotal * 0.18m; //m-decimal or else treated as double

            decimal grandTotal = subTotal + tax - discount;


            var bill = new Bill
            {
                BillNumber = "INV-" + DateTime.Now.ToString("yyyyMMddHHmmss"),

                CustomerId = customer.Id,

                StaffId = staffId.Value,

                BillDate = DateTime.Now,

                SubTotal = subTotal,

                Discount = discount,

                Tax = tax,

                GrandTotal = grandTotal,

                PaymentStatus = "Success",

                BillItems = billItems //objects for each service in the bill
            };


            _context.Bills.Add(bill); //add this bill object to the Bills table

            _context.SaveChanges();


            return Json(new
            {
                success = true,id = bill.Id
            });
        }

        [HttpGet] //bill list
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Staff" && role != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }

            if (role == "Admin")
            {
                var allBills = _context.Bills
                    .Include(x => x.Customer)
                    .Include(x => x.Staff)
                    .OrderByDescending(x => x.BillDate)
                    .ToList();

                return View(allBills);
            }


            if (role == "Staff")
            {
                var staffId = HttpContext.Session.GetInt32("UserId");

                if (staffId == null)
                {
                    return Unauthorized();
                }


                var staffBills = _context.Bills
                    .Include(x => x.Customer)
                    .Include(x => x.Staff)
                    .Where(x => x.StaffId == staffId.Value)
                    .OrderByDescending(x => x.BillDate)
                    .ToList();
                     //Index / List page → (List<Bill>) because you're showing many records.
                    //Details page → send one object(Bill) because you're showing one record.

                return View(staffBills);
            }


            return Forbid();
        }


        [HttpGet] //one specific bill
        public IActionResult Details(int id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Staff" && role != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }


            var bill = _context.Bills
                .Include(x => x.Customer)
                .Include(x => x.Staff)
                .Include(x => x.BillItems)
                .ThenInclude(x => x.Service)
                .FirstOrDefault(x => x.Id == id);


            if (bill == null) //change the return type to IActionResult because we are returning different types of responses
            {
                return NotFound();
            }


            if (role == "Admin")
            {
                return View(bill);
            }


            if (role == "Staff")
            {
                var staffId =HttpContext.Session.GetInt32("UserId");

                if (staffId == null)
                {
                    return Unauthorized();
                }


                if (bill.StaffId != staffId.Value)
                {
                    return Forbid();
                }


                return View(bill);
            }


            return Forbid();
        }

        [HttpGet]
        public IActionResult Pdf(int id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Staff" && role != "Admin")
            {
                return RedirectToAction("Login", "Users");
            }


            var bill = _context.Bills
                .Include(x => x.Customer)
                .Include(x => x.Staff)
                .Include(x => x.BillItems)
                .ThenInclude(x => x.Service)
                .FirstOrDefault(x => x.Id == id);


            if (bill == null)
            {
                return NotFound();
            }

            if (role == "Admin")
            {
                return GeneratePdf(bill);
            }


            if (role == "Staff")
            {
                var staffId = HttpContext.Session.GetInt32("UserId");

                if (staffId == null)
                {
                    return Unauthorized();
                }


                if (bill.StaffId != staffId.Value)
                {
                    return Forbid();
                }


                return GeneratePdf(bill);
            }


            return Forbid();
        }

        //Helper/private method: → used internally by your controller to avoid repeating code
        //generate the PDF for both Admin and Staff.Without a helper, you would have to repeat this code:
        // and the browser doesn't directly request:/Bills/GeneratePdf So it is not an action.
      
        private IActionResult GeneratePdf(Bill bill)
        {
            var company = _context.Companies.FirstOrDefault();

            if (company == null)
            {
                return NotFound("Company details not found.");
            }

            var pdfService = new PdfService();

            byte[] pdf =pdfService.GenerateInvoice(bill, company); //generated pdf returned as binary data

            return File( pdf, "application/pdf",$"{bill.BillNumber}.pdf" //send pdf to browser(PDF is a file made of binary data not normal text)
            );
        }
    }
}