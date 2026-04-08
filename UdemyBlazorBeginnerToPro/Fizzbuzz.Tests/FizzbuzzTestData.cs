namespace Fizzbuzz.Tests;

internal class FizzbuzzTestData : TheoryData<int, int, int, int, string[]>
{
    public FizzbuzzTestData()
    {
        Add(
            1,
            15,
            3,
            5,
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
            ]
        );

        Add(1, 1, 3, 5, ["1"]);
        Add(1, 1, 1, 2, ["Fizz"]);
        Add(1, 1, 2, 1, ["Buzz"]);
        Add(1, 1, 1, 1, ["FizzBuzz"]);
    }
}
