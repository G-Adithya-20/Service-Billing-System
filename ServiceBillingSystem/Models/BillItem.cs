using System.ComponentModel.DataAnnotations;
namespace ServiceBillingSystem.Models;
public class BillItem
{
    public int Id { get; set; }

    [Required]
    public int BillId { get; set; }

    public Bill? Bill { get; set; }

    [Required(ErrorMessage = "Service is required")]
    public int ServiceId { get; set; }

    public Service? Service { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Unit price is required")]
    [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 1")]
    public decimal UnitPrice { get; set; }

    [Range(1, double.MaxValue, ErrorMessage = "Total must be greater than 1")]
    public decimal Total { get; set; }
}