using LoanShark.Models;
using LoanShark.Services;

namespace LoanShark.Tests
{
    public class LoanCalculatorTests
    {
        [Theory]
        [InlineData(25000, 5, 60, 471.78, 3306.85, 28306.85)]
        public void Can_Calculate_Monthly_Payments_And_Totals(
            double amount,
            double rate,
            int termMonths,
            double expectedPayment,
            double expectedTotalInterest,
            double expectedTotalCost
        )
        {
            var model = new LoanInputModel
            {
                Amount = amount,
                Rate = rate,
                TermMonths = termMonths,
            };

            var calculator = new LoanCalculator();
            calculator.CalculateLoan(model);

            Assert.Equal(expectedPayment, Math.Round(calculator.MonthlyPayment, 2));
            Assert.Equal(expectedTotalInterest, Math.Round(calculator.TotalInterest, 2));
            Assert.Equal(expectedTotalCost, Math.Round(calculator.TotalCost, 2));
        }
    }
}
