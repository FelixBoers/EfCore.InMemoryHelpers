using Xunit;
using Xunit.Abstractions;
using System.Linq;
using EfCore.InMemoryHelpers;
using Microsoft.EntityFrameworkCore;

public class IdResetTests : TestBase
{
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

    public IdResetTests(ITestOutputHelper output) :
        base(output)
    {
    }
    public class TestEntity
    {
        public int Id { get; set; }
        public string Property { get; set; }
    }

    class TestDataContext : DbContext
    {
        public DbSet<TestEntity> TestEntities { get; set; }

        public TestDataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<TestEntity>();
            entity.Property(b => b.Property)
                .IsRequired();
        }
    }
}