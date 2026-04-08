using Fizzbuzz.Services;

namespace Fizzbuzz.Tests
{
    public class FizzbuzzServiceTests
    {
        [Theory]
        [ClassData(typeof(FizzbuzzTestData))]
        public void GetFizzbuzz_Returns_Expected_List(
            int minInclusive,
            int maxInclusive,
            int fizzNumber,
            int buzzNumber,
            string[] expectedResult
        )
        {
            var result = FizzbuzzService
                .GetFizzbuzz(minInclusive, maxInclusive, fizzNumber, buzzNumber)
                .ToList();

            Assert.True(result.SequenceEqual(expectedResult));
        }
    }
}
