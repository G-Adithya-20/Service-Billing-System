namespace ServiceBillingSystem.Models;
public class BillCreateRequest
{
    public string CustomerName { get; set; } = "";

    public decimal Discount { get; set; }

    public List<BillItemRequest> Items { get; set; } = new();
}

public class BillItemRequest
{
    public int ServiceId { get; set; }

    public int Quantity { get; set; }
}