using System.ComponentModel.DataAnnotations;
using Fizzbuzz.Services;

namespace Fizzbuzz.Models
{
    internal sealed class FizzbuzzInputModel
    {
        [Required(ErrorMessage = "Please enter buzz value.")]
        [Range(
            FizzbuzzService.BuzzNumberMinValue,
            100,
            ErrorMessage = "Buzz value must be between 0 and 100."
        )]
        public int BuzzNumber { get; set; } = 5;

        [Required(ErrorMessage = "Please enter fizz value.")]
        [Range(
            FizzbuzzService.FizzNumberMinValue,
            100,
            ErrorMessage = "Fizz value must be between 0 and 100."
        )]
        public int FizzNumber { get; set; } = 3;

        [Required(ErrorMessage = "Please enter stop value.")]
        [Range(
            1,
            FizzbuzzService.MaxInclusiveMaxValue,
            ErrorMessage = "Stop value must be between 1 and 100."
        )]
        public int MaxInclusive { get; set; } = 100;

        [Required(ErrorMessage = "Please enter start value.")]
        [Range(
            FizzbuzzService.MinInclusiveMinValue,
            100,
            ErrorMessage = "Start value must be between 1 and 100."
        )]
        public int MinInclusive { get; set; } = 5;
    }
}
