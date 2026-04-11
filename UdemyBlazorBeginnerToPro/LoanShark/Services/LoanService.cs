using LoanShark.Models;

namespace LoanShark.Services
{
    public class LoanService
    {
        public double MonthlyPayment { get; private set; }
        public double TotalCost { get; private set; }
        public double TotalInterest { get; private set; }

        public void CalculateLoan(LoanInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            CalculateMonthlyPayment(model);
            CalculateTotals(model);
        }

        private void CalculateMonthlyPayment(LoanInputModel model)
        {
            MonthlyPayment =
                model.Amount
                * (model.Rate / 1200)
                / (1 - Math.Pow(1 + model.Rate / 1200, -model.TermMonths));
        }

        private void CalculateTotals(LoanInputModel model)
        {
            TotalInterest = (MonthlyPayment * model.TermMonths) - model.Amount;
            TotalCost = TotalInterest + model.Amount;
        }
    }
}
