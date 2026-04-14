namespace LoanShark.Domain
{
    public sealed class PaymentScheduleItem
    {
        public double Interest { get; set; }
        public int Month { get; set; }
        public double Payment { get; set; }
        public double Principal { get; set; }
        public double RemainingBalance { get; set; }
        public double TotalInterest { get; set; }
    }
}
