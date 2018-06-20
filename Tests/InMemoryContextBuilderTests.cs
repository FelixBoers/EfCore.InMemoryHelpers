using Xunit;
using Xunit.Abstractions;
using System.Linq;
using EfCore.InMemoryHelpers;

public class InMemoryContextBuilderTests : TestBase
{
    [Fact]
    public void GetInMemoryContext()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var user = new TestEntity
            {
                Property = "prop"
            };
            context.Add(user);
            context.SaveChanges();
            var item = context.TestEntities.ToList();
            Assert.Single(item);
        }
    }

    public InMemoryContextBuilderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}