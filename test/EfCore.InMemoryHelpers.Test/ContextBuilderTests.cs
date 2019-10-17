using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace EfCore.InMemoryHelpers.Test
{
    public class ContextBuilderTests : TestBase
    {
        public ContextBuilderTests(ITestOutputHelper output)
            :
            base(output)
        { }

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
        public void GetInMemoryContextWithSpecifiedDbName()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>("MyDatabase"))
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
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(b => b.Property)
                    .IsRequired();
            }
        }
    }
}