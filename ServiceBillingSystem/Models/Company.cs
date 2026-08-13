using System.ComponentModel.DataAnnotations;
namespace ServiceBillingSystem.Models
{
    public class Company
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        public string name { get; set; } = "";

        [Required(ErrorMessage = "Address is required")]
        public string address { get; set; } = "";

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[6-9]\d{9}$",
            ErrorMessage = "Enter a valid 10-digit phone number")]
        public string phonenumber { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string email { get; set; } = "";
    }
}