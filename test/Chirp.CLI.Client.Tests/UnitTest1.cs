using CommandLine;
using SimpleDB;
using Chirp.CLI.Client;
using Xunit;
namespace Chirp.CLI.Client.Tests;

public class UnitTest1
{
    [Theory]
    [InlineData("user1","message 1",32332423)]
    public void Test1(string user, string message, double time)
    {
       // return Parser.Default.ParseArguments<>
        var p = new Program();
        
    }
    [Fact]
    public void unixConversionTest()
    {
        //Arrange
        var unixTimestamp = new DateTimeOffset(2025, 9, 18, 12, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds();
        var cheep = new Cheep("Andreas","Hello World!", unixTimestamp);

        var expectedTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).ToLocalTime().ToString("yyyy-MM-dd HH:mm");
        var expectedOutput = $"Andreas @ {expectedTime}: Hello World!";
        
        //Act
        var result = UserInterface.FormatCheep(cheep);
        //Assert
        Assert.Equal(expectedOutput, result);
    }
}  