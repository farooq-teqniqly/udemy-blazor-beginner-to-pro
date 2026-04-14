using LoanShark.Models;
using LoanShark.Services;

namespace LoanShark.Tests
{
    public class LoanServiceTests
    {
        [Theory]
        [InlineData(25000, 5, 60, 471.78, 3306.85, 28306.85)]
        [InlineData(1000, 0, 10, 100, 0, 1000)]
        public void CalculateLoan_Calculates_Monthly_Payments_And_Totals(
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

            var loan = new LoanService();
            loan.CalculateLoan(model);

            Assert.Equal(expectedPayment, Math.Round(loan.MonthlyPayment, 2));
            Assert.Equal(expectedTotalInterest, Math.Round(loan.TotalInterest, 2));
            Assert.Equal(expectedTotalCost, Math.Round(loan.TotalCost, 2));
        }

        [Fact]
        public void CalculateLoan_Calculates_Payment_Schedule_And_First_Schedule_Is_Correct()
        {
            var model = new LoanInputModel
            {
                Amount = 25000,
                Rate = 5,
                TermMonths = 60,
            };

            var loan = new LoanService();
            loan.CalculateLoan(model);

            var schedule = loan.PaymentScheduleViewModel;

            Assert.Equal(model.TermMonths, schedule.Count);

            Assert.True(
                schedule.All(m =>
                    Math.Abs(Math.Round(m.Payment, 2) - Math.Round(loan.MonthlyPayment, 2)) < 0.01
                )
            );

            var firstPaymentSchedule = schedule.First();

            Assert.Equal(1, firstPaymentSchedule.Month);
            Assert.Equal(367.61, Math.Round(firstPaymentSchedule.Principal, 2));
            Assert.Equal(104.17, Math.Round(firstPaymentSchedule.Interest, 2));
            Assert.Equal(104.17, Math.Round(firstPaymentSchedule.TotalInterest, 2));
            Assert.Equal(24632.39, Math.Round(firstPaymentSchedule.Balance, 2));
        }

        [Fact]
        public void CalculateLoan_Calculates_Payment_Schedule_And_Last_Schedule_Is_Correct()
        {
            var model = new LoanInputModel
            {
                Amount = 25000,
                Rate = 5,
                TermMonths = 60,
            };

            var loan = new LoanService();
            loan.CalculateLoan(model);

            var schedule = loan.PaymentScheduleViewModel;

            var lastPaymentSchedule = schedule.Last();

            Assert.Equal(60, lastPaymentSchedule.Month);
            Assert.Equal(469.82, Math.Round(lastPaymentSchedule.Principal, 2));
            Assert.Equal(1.96, Math.Round(lastPaymentSchedule.Interest, 2));
            Assert.Equal(
                Math.Round(loan.TotalInterest, 2),
                Math.Round(lastPaymentSchedule.TotalInterest, 2)
            );
            Assert.Equal(0, lastPaymentSchedule.Balance);
        }

        [Fact]
        public void CalculateLoan_Calculates_Payment_Schedule_And_Schedule_Has_Correct_Number_Of_Payments()
        {
            var model = new LoanInputModel
            {
                Amount = 25000,
                Rate = 5,
                TermMonths = 60,
            };

            var loan = new LoanService();
            loan.CalculateLoan(model);

            var schedule = loan.PaymentScheduleViewModel;

            Assert.Equal(model.TermMonths, schedule.Count);
        }
    }
}
