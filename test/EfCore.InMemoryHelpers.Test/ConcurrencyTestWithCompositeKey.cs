﻿using ApprovalTests;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit;
using Xunit.Abstractions;

namespace EfCore.InMemoryHelpers.Test
{
    public class ConcurrencyTestWithCompositeKey : TestBase
    {
        public ConcurrencyTestWithCompositeKey(ITestOutputHelper output)
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
                    Id1 = entity.Id1,
                    Id2 = entity.Id2,
                    Property = "Something new"
                };
                context.Attach(update);
                context.Entry(update).Property("Property").IsModified = true;
                var exception = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
                Approvals.Verify(exception.Message);
            }
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
                    Id1 = 1,
                    Id2 = 1,
                    Property = "prop"
                };
                var entity2 = new TestEntity
                {
                    Id1 = 1,
                    Id2 = 2,
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
                var firstTimestamp = entity.Timestamp.GetGuid();
                entity.Property = "Something new";

                context.SaveChanges();
                Assert.NotEqual(firstTimestamp, entity.Timestamp.GetGuid());
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
                    Id1 = entity.Id1,
                    Id2 = entity.Id2,
                    Property = "Something new",
                    Timestamp = RowVersion.New()
                };
                context.Attach(update);
                context.Entry(update).Property("Property").IsModified = true;
                var exception = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
                Approvals.Verify(exception.Message);
            }
        }

        public class TestEntity
        {
            public int Id1 { get; set; }

            public int Id2 { get; set; }

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
                entity.Property(p => p.Id1)
                    .ValueGeneratedOnAdd();

                entity.Property(p => p.Id2)
                    .ValueGeneratedOnAdd();

                entity.Property(p => p.Timestamp)
                    .IsRowVersion();

                entity.HasKey(p => new { p.Id1, p.Id2 });
            }
        }
    }
}