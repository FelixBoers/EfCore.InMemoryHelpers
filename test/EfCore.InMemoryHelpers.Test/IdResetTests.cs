using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore.InMemoryHelpers.Test
{
    public class IdResetTests : TestBase
    {
        public IdResetTests(ITestOutputHelper output)
            :
            base(output)
        { }

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

        public class TestEntity
        {
            public int Id { get; set; }
            public string Property { get; set; }
        }

        private class TestDataContext : DbContext
        {
            public TestDataContext(DbContextOptions options)
                : base(options)
            { }

            public DbSet<TestEntity> TestEntities { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                var entity = modelBuilder.Entity<TestEntity>();
                entity.Property(b => b.Property)
                    .IsRequired();
            }
        }
    }
}