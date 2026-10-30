using NbaHub.Domain.Players;

namespace NbaHub.Domain.Tests.Players;

public class HeightTests
{
    [Theory]
    [InlineData("6-2", 74)]
    [InlineData("7-0", 84)]
    [InlineData(" 5-11 ", 71)]
    public void TryParse_reads_feet_and_inches_format(string value, int expectedInches)
    {
        Height.TryParse(value, out var height).ShouldBeTrue();

        height.TotalInches.ShouldBe(expectedInches);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("6")]
    [InlineData("6-12")]
    [InlineData("six-two")]
    [InlineData("6-2-1")]
    [InlineData("-6-2")]
    [InlineData("2-0")]
    public void TryParse_rejects_invalid_values(string? value)
    {
        Height.TryParse(value, out var height).ShouldBeFalse();

        height.ShouldBe(default);
    }

    [Fact]
    public void ToString_uses_feet_and_inches_notation()
    {
        Height.FromFeetAndInches(6, 9).ToString().ShouldBe("6'9\"");
    }

    [Fact]
    public void Centimetres_is_rounded_to_one_decimal_place()
    {
        Height.FromInches(74).Centimetres.ShouldBe(188.0);
    }

    [Theory]
    [InlineData(47)]
    [InlineData(109)]
    public void FromInches_rejects_heights_outside_the_realistic_range(int inches)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => Height.FromInches(inches));
    }

    [Fact]
    public void Heights_with_the_same_inches_are_equal()
    {
        Height.FromFeetAndInches(6, 2).ShouldBe(Height.FromInches(74));
    }
}
