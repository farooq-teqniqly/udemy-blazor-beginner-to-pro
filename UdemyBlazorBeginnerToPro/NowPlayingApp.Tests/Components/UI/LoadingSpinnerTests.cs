using NowPlayingApp.Components.UI;

#pragma warning disable BL0005

namespace NowPlayingApp.Tests.Components.UI;

public class LoadingSpinnerTests
{
    [Fact]
    public void Default_parameters_use_expected_label_and_size()
    {
        var sut = new LoadingSpinner();

        Assert.Equal("Loading...", sut.Label);
        Assert.Equal(2, sut.SizeRem);
    }

    [Fact]
    public void Custom_label_and_size_are_returned()
    {
        var sut = new LoadingSpinner { Label = "Loading poster", SizeRem = 3 };

        Assert.Equal("Loading poster", sut.Label);
        Assert.Equal(3, sut.SizeRem);
    }
}
