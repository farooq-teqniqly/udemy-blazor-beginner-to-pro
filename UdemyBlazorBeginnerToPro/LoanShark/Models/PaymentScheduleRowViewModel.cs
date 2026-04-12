namespace LoanShark.Models;

public sealed class PaymentScheduleRowViewModel
{
    public double Balance { get; set; }
    public double Interest { get; set; }
    public int Month { get; set; }
    public double Payment { get; set; }
    public double Principal { get; set; }
    public double TotalInterest { get; set; }
}
