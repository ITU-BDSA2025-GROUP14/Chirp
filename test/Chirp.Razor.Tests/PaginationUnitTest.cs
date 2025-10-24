using Xunit;

public class PaginationService
{
    public IEnumerable<T> GetPage<T>(IEnumerable<T> items, int pageNumber, int pageSize)
    {
        return items.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}

public class PaginationServiceTests
{
    [Fact]
    public void GetPage_ReturnsCorrectSubset()
    {
        // Arrange
        var items = Enumerable.Range(1, 10);
        var service = new PaginationService();

        // Act
        var result = service.GetPage(items, 2, 3);

        // Assert
        Assert.Equal(new[] { 4, 5, 6 }, result);
    }
}