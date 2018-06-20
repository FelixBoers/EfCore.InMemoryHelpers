using System;
using Xunit;
using Xunit.Abstractions;
using System.Linq;
using ApprovalTests;
using EfCore.InMemoryHelpers;

public class ContextBuilderTests : TestBase
{
    [Fact]
    public void GetInMemoryContext()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity = new TestEntity
            {
                Property = "prop"
            };
            context.Add(entity);
            context.SaveChanges();
            var item = context.TestEntities.ToList();
            Assert.Single(item);
        }
    }

    [Fact]
    public void UniqueIndexThrows()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity1 = new TestEntity
            {
                Property = "prop"
            };
            context.Add(entity1);
            var user2 = new TestEntity
            {
                Property = "prop"
            };
            context.Add(user2);
            var exception = Assert.Throws<Exception>(()=> context.SaveChanges());
            Approvals.Verify(exception.Message);
        }
    }

    [Fact]
    public void AssertIdIsReset()
    {
        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity = new TestEntity
            {
                Property = "prop1"
            };
            context.Add(entity);
            context.SaveChanges();
            var id = context.TestEntities.Single().Id;
            Assert.Equal(1, id);
        }

        using (var context = InMemoryContextBuilder.Build<TestDataContext>())
        {
            var entity = new TestEntity
            {
                Property = "prop2"
            };
            context.Add(entity);
            context.SaveChanges();
            var id = context.TestEntities.Single().Id;
            Assert.Equal(1, id);
        }
    }

    public ContextBuilderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}