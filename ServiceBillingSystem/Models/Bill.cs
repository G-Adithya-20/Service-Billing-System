using System.ComponentModel.DataAnnotations;
namespace ServiceBillingSystem.Models;
public class Bill
{
    public int Id { get; set; }

    [Required]
    public string BillNumber { get; set; } = "";

    [Required(ErrorMessage = "Customer is required")]
    public int CustomerId { get; set; }

    public Customer? Customer { get; set; }

    [Required(ErrorMessage = "Staff is required")]
    public int StaffId { get; set; }

    public User? Staff { get; set; }

    [Required]
    public DateTime BillDate { get; set; }

    [Range(1, double.MaxValue)]
    public decimal SubTotal { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Discount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Tax { get; set; }

    [Range(1, double.MaxValue)]
    public decimal GrandTotal { get; set; }

    [Required(ErrorMessage = "Payment status is required")]
    public string PaymentStatus { get; set; } = "";

    public ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();
}