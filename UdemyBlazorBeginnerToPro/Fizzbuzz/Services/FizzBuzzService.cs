using System.Text;

namespace Fizzbuzz.Services
{
    /// <summary>
    /// Provides static methods for generating FizzBuzz sequences.
    /// A FizzBuzz sequence replaces numbers divisible by specified values with "Fizz", "Buzz", or "FizzBuzz".
    /// </summary>
    internal static class FizzbuzzService
    {
        internal const int MinInclusiveMinValue = 1;
        internal const int MaxInclusiveMaxValue = 100;
        internal const int FizzNumberMinValue = 1;
        internal const int BuzzNumberMinValue = 1;

        /// <summary>
        /// Generates a sequence of FizzBuzz values for numbers in the specified range.
        /// Numbers divisible by fizzNumber return "Fizz", numbers divisible by buzzNumber return "Buzz",
        /// numbers divisible by both return "FizzBuzz", otherwise the number itself is returned as a string.
        /// </summary>
        /// <param name="minInclusive">The minimum number in the range (inclusive). Must be greater than or equal to 1.</param>
        /// <param name="maxInclusive">The maximum number in the range (inclusive). Must be less than or equal to 100.</param>
        /// <param name="fizzNumber">The divisor for "Fizz" values. Must be non-negative.</param>
        /// <param name="buzzNumber">The divisor for "Buzz" values. Must be non-negative.</param>
        /// <returns>An enumerable sequence of FizzBuzz strings corresponding to numbers from 1 to maxInclusive.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when minInclusive is less than 1, maxInclusive is greater than 100,
        /// fizzNumber is negative, or buzzNumber is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when minInclusive is greater than maxInclusive.
        /// </exception>
        internal static IEnumerable<string> GetFizzbuzz(
            int minInclusive,
            int maxInclusive,
            int fizzNumber,
            int buzzNumber
        )
        {
            EnsureArgs(minInclusive, maxInclusive, fizzNumber, buzzNumber);

            for (var i = 1; i <= maxInclusive; i++)
            {
                yield return GetFizzbuzzValue(i, fizzNumber, buzzNumber);
            }
        }

        private static void EnsureArgs(
            int minInclusive,
            int maxInclusive,
            int fizzNumber,
            int buzzNumber
        )
        {
            if (minInclusive < MinInclusiveMinValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(minInclusive),
                    $"Value must be greater than or equal to {MinInclusiveMinValue}."
                );
            }

            if (maxInclusive > MaxInclusiveMaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxInclusive),
                    $"Value must be less than or equal to {MaxInclusiveMaxValue}."
                );
            }

            if (minInclusive > maxInclusive)
            {
                throw new ArgumentException(
                    "minInclusive must be less than or equal to maxInclusive."
                );
            }

            if (fizzNumber < FizzNumberMinValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(fizzNumber),
                    $"fizzNumber must be greater than or equal to {FizzNumberMinValue}."
                );
            }

            if (buzzNumber < BuzzNumberMinValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(buzzNumber),
                    $"buzzNumber must be greater than or equal to {BuzzNumberMinValue}."
                );
            }
        }

        private static string GetFizzbuzzValue(int i, int fizzNumber, int buzzNumber)
        {
            var isFizz = i % fizzNumber == 0;
            var isBuzz = i % buzzNumber == 0;
            var sb = new StringBuilder();

            if (!isFizz && !isBuzz)
            {
                return i.ToString();
            }

            if (isFizz)
            {
                sb.Append("Fizz");
            }

            if (isBuzz)
            {
                sb.Append("Buzz");
            }

            return sb.ToString();
        }
    }
}
