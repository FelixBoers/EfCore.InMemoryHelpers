using System;
using System.Linq;
using ApprovalTests;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore.InMemoryHelpers.Test
{
    public class ConcurrencyTests : TestBase
    {
        public ConcurrencyTests(ITestOutputHelper output)
            :
            base(output)
        { }

        [Fact]
        public void NullConflictThrows()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity = new TestEntity
                {
                    Property = "prop"
                };
                context.Add(entity);
                context.SaveChanges();
                var update = new TestEntity
                {
                    Id = entity.Id,
                    Property = "Something new"
                };
                context.Entry(update).Property("Property").IsModified = true;
                var exception = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
                Approvals.Verify(exception.Message);
            }
        }

        [Fact]
        public void ReusabilitySucceeds()
        {
            const string dbName = "MyDatabase";
            const string value = "prop";
            using (var context = InMemoryContextBuilder.Build<TestDataContext>(dbName))
            {
                var entity = new TestEntity
                {
                    Property = value
                };
                context.Add(entity);
                context.SaveChanges();
            }

            TestEntity res;
            using (var context = InMemoryContextBuilder.Build<TestDataContext>(dbName))
            {
                res = context.TestEntities.First(e => e.Property == value);
            }

            Assert.NotNull(res);
        }

        [Fact]
        public void SetRowVersionToNullThrows()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity = new TestEntity
                {
                    Property = "prop"
                };
                context.Add(entity);
                context.SaveChanges();
                entity.Timestamp = null;
                var exception = Assert.Throws<Exception>(() => context.SaveChanges());
                Approvals.Verify(exception.Message);
            }
        }

        [Fact]
        public void UpdateMultipleSameEntity()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity1 = new TestEntity
                {
                    Property = "prop"
                };
                var entity2 = new TestEntity
                {
                    Property = "prop"
                };
                context.AddRange(entity1, entity2);
                context.SaveChanges();
                entity1.Property = "Something new";
                entity2.Property = "Something new";
                context.SaveChanges();
            }
        }

        [Fact]
        public void UpdateSameEntity()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity = new TestEntity
                {
                    Property = "prop"
                };
                context.Add(entity);
                context.SaveChanges();
                entity.Property = "Something new";
                context.SaveChanges();
            }
        }

        [Fact]
        public void UpdateSucceeds()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity = new TestEntity
                {
                    Property = "prop"
                };
                context.Add(entity);
                context.SaveChanges();
                var firstTimestamp = entity.Timestamp;
                var update = new TestEntity
                {
                    Id = entity.Id,
                    Property = "Something new",
                    Timestamp = firstTimestamp
                };
                context.Entry(update).Property("Property").IsModified = true;
                context.SaveChanges();
                Assert.NotEqual(firstTimestamp.GetGuid(), update.Timestamp.GetGuid());
            }
        }

        [Fact]
        public void ValueConflictThrows()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity = new TestEntity
                {
                    Property = "prop"
                };
                context.Add(entity);
                context.SaveChanges();
                var update = new TestEntity
                {
                    Id = entity.Id,
                    Property = "Something new",
                    Timestamp = RowVersion.New()
                };
                context.Entry(update).Property("Property").IsModified = true;
                var exception = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
                Approvals.Verify(exception.Message);
            }
        }

        public class TestEntity
        {
            public int Id { get; set; }
            public string Property { get; set; }
            public byte[] Timestamp { get; set; }
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
                entity.Property(p => p.Timestamp)
                    .IsRowVersion();
            }
        }
    }
}