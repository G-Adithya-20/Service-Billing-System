using System.ComponentModel.DataAnnotations;
namespace ServiceBillingSystem.Models;
public class Service
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Service name is required")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = "";

    [Required(ErrorMessage = "Price is required")]
    [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 1")]
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true; //soft disable
}
