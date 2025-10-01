using SimpleDB;
using System.Collections;

using Xunit;
namespace Chirp.CSVDB.Tests;

public class SimpleDB_integrationTest
{
    [Theory]
    [InlineData("erw", "2131", 123)]
    public void test_read_and_write_from_csv(String username, String message, long timestamp)
    {
        //Arrange
        CSVDatabase<Cheep> db = CSVDatabase<Cheep>.GetInstance();
        Cheep cheep = new Cheep(username, message, timestamp);
        
        //Act
        db.Store(cheep);
        var cheeps = db.Read(Int32.MaxValue);
        
        //Assert
        Assert.Equal(cheeps.Last(),cheep);
    }
}
