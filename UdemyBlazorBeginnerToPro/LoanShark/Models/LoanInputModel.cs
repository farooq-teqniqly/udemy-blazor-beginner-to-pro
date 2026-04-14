using System.ComponentModel.DataAnnotations;

namespace LoanShark.Models
{
    public sealed class LoanInputModel
    {
        [Required(ErrorMessage = "Loan amount is required.")]
        [Range(100, 1000000, ErrorMessage = "Loan amount must be between $100 and $1,000,000.")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Interest rate is required.")]
        [Range(0.0, 100.0, ErrorMessage = "Interest rate must be between 0% and 100%.")]
        public double Rate { get; set; }

        [Required(ErrorMessage = "Loan term in months is required.")]
        [Range(1, 1200, ErrorMessage = "Loan term must be between 1 and 1200 months.")]
        public int TermMonths { get; set; }
    }
}
