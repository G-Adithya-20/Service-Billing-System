using System.ComponentModel.DataAnnotations;
namespace ServiceBillingSystem.Models;
public class Customer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Customer name is required")]
    public string Name { get; set; } = "";

    [RegularExpression(@"^[6-9]\d{9}$",
    ErrorMessage = "Enter a valid 10-digit phone number")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = "";
}