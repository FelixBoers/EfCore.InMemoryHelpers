using ApprovalTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using Xunit;
using Xunit.Abstractions;

namespace EfCore.InMemoryHelpers.Test
{
    public class ConcurrencyWithLongTests : TestBase
    {
        public ConcurrencyWithLongTests(ITestOutputHelper output)
            :
            base(output)
        { }

        [Fact]
        public void NullConflictThrows()
        {
            var entity = new TestEntity
            {
                Property = "prop"
            };

            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var update = new TestEntity
                {
                    Id = entity.Id,
                    Property = "Something new"
                };
                context.Entry(update).Property("Property").IsModified = true;
                var exception = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
                Approvals.Verify(exception.Message.Replace(entity.Timestamp.ToString(), ""));
            }
        }

        [Fact]
        public void SetRowVersionToZeroThrows()
        {
            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var entity = new TestEntity
                {
                    Property = "prop"
                };
                context.Add(entity);
                context.SaveChanges();
                entity.Timestamp = 0;
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
                entity.Property = "Something new";
                context.SaveChanges();
                Assert.NotEqual(firstTimestamp, entity.Timestamp);
            }
        }

        [Fact]
        public void ValueConflictThrows()
        {
            var entity = new TestEntity
            {
                Property = "prop"
            };

            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = InMemoryContextBuilder.Build<TestDataContext>())
            {
                var update = new TestEntity
                {
                    Id = entity.Id,
                    Property = "Something new",
                    Timestamp = 1
                };
                context.Attach(update);
                context.Entry(update).Property("Property").IsModified = true;
                var exception = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
                Approvals.Verify(exception.Message.Replace(entity.Timestamp.ToString(), ""));
            }
        }

        public class TestEntity
        {
            public int Id { get; set; }

            public string Property { get; set; }

            public ulong Timestamp { get; set; }
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

                entity.Property(p => p.Timestamp)
                    .HasConversion(new NumberToBytesConverter<ulong>())
                    .IsRowVersion()
                    .IsRequired()
                    .HasColumnType("RowVersion");
            }
        }
    }
}