namespace LoanShark.Models;

public sealed class LoanViewModel
{
    public double MonthlyPayment { get; set; }
    public List<PaymentScheduleRowViewModel> Payments { get; set; } = [];
    public double TotalCost { get; set; }
    public double TotalInterest { get; set; }
    public double TotalPrincipal { get; set; }
}
