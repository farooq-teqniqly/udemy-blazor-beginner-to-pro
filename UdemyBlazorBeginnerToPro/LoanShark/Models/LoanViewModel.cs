namespace LoanShark.Models;

public sealed class LoanViewModel
{
    public double MonthlyPayment { get; set; }
    public List<PaymentViewModel> Payments { get; set; } = [];
    public double TotalCost { get; set; }
    public double TotalInterest { get; set; }
    public double TotalPrincipal { get; set; }
}
