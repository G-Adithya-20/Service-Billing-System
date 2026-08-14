using System.ComponentModel.DataAnnotations;
namespace ServiceBillingSystem.Models;
public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must contain at least 6 characters")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; } = "Staff";

    public bool IsInitialAdmin { get; set; }

    public bool IsActive { get; set; } = true;
}