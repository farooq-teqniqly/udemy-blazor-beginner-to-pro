using Fizzbuzz.Services;

namespace Fizzbuzz.Tests
{
    public class FizzbuzzServiceTests
    {
        [Fact]
        public void GetFizzbuzz_Returns_Expected_List()
        {
            var result = FizzbuzzService.GetFizzbuzz(1, 15, 3, 5).ToList();

            string[] expected =
            [
                "1",
                "2",
                "Fizz",
                "4",
                "Buzz",
                "Fizz",
                "7",
                "8",
                "Fizz",
                "Buzz",
                "11",
                "Fizz",
                "13",
                "14",
                "FizzBuzz",
            ];

            Assert.True(result.SequenceEqual(expected));
        }
    }
}
