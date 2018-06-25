using System;
using Xunit;
using Xunit.Abstractions;
using ApprovalTests;
using EfCore.InMemoryHelpers;
using Microsoft.EntityFrameworkCore;

public class UniqueIndexTests : TestBase
{
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

    public UniqueIndexTests(ITestOutputHelper output) :
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

            entity.HasIndex(u => u.Property)
                .IsUnique();
        }
    }
}