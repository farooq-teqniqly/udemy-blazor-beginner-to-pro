using NowPlayingApp.Components.UI;

#pragma warning disable BL0005

namespace NowPlayingApp.Tests.Components.UI;

public class LoadingSpinnerTests
{
    [Fact]
    public void Default_parameters_use_expected_label_and_size()
    {
        // Arrange
        var sut = new LoadingSpinner();

        // Act
        var label = sut.Label;
        var sizeRem = sut.SizeRem;

        // Assert
        Assert.Equal("Loading...", label);
        Assert.Equal(2, sizeRem);
    }

    [Fact]
    public void Custom_label_and_size_are_returned()
    {
        // Arrange
        var sut = new LoadingSpinner { Label = "Loading poster", SizeRem = 3 };

        // Act
        var label = sut.Label;
        var sizeRem = sut.SizeRem;

        // Assert
        Assert.Equal("Loading poster", label);
        Assert.Equal(3, sizeRem);
    }
}
