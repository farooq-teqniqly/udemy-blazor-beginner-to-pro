using LoanShark.Domain;
using LoanShark.Models;

namespace LoanShark.Services
{
    public class LoanService
    {
        public double MonthlyPayment { get; private set; }
        public List<PaymentScheduleRowViewModel> PaymentScheduleViewModel { get; set; } = [];
        public double TotalCost { get; private set; }
        public double TotalInterest { get; private set; }

        public void CalculateLoan(LoanInputModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            CalculateMonthlyPayment(model);
            CalculateTotals(model);
            CalculatePaymentSchedule(model);
        }

        private void CalculateMonthlyPayment(LoanInputModel model)
        {
            MonthlyPayment =
                model.Amount
                * (model.Rate / 1200)
                / (1 - Math.Pow(1 + model.Rate / 1200, -model.TermMonths));
        }

        private void CalculatePaymentSchedule(LoanInputModel model)
        {
            var monthlyPayment = MonthlyPayment;
            var balance = model.Amount;
            var rate = model.Rate / 1200;
            var totalInterest = 0.0;

            for (var month = 1; month <= model.TermMonths; month++)
            {
                var interest = balance * rate;
                var principal = monthlyPayment - interest;

                if (month == model.TermMonths)
                {
                    principal = balance;
                    monthlyPayment = principal + interest;
                }

                var remainingBalance = balance - principal;
                totalInterest += interest;

                var viewModel = new PaymentScheduleRowViewModel
                {
                    Balance = remainingBalance,
                    Interest = interest,
                    Month = month,
                    Payment = monthlyPayment,
                    Principal = principal,
                    TotalInterest = totalInterest,
                };

                PaymentScheduleViewModel.Add(viewModel);

                balance = remainingBalance;
            }
        }

        private void CalculateTotals(LoanInputModel model)
        {
            TotalInterest = (MonthlyPayment * model.TermMonths) - model.Amount;
            TotalCost = TotalInterest + model.Amount;
        }
    }
}
