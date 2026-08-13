namespace ServiceBillingSystem.Models;
//send data from controller to view
//represent complete data
public class AdminDashboardViewModel
{
    public int TotalStaff { get; set; }

    public int TotalCustomers { get; set; }

    public int TotalServices { get; set; }

    public int TotalBills { get; set; }
}