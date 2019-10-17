using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace EfCore.InMemoryHelpers
{
    public static class InMemoryContextBuilder
    {
        private static readonly HashSet<string> existingDatabases = new HashSet<string>();

        public static TContext Build<TContext>(bool enableSensitiveDataLogging = true)
            where TContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TContext>();
            builder.EnableSensitiveDataLogging(enableSensitiveDataLogging);
            return Build<TContext>(builder);
        }

        public static TContext Build<TContext>(DbContextOptionsBuilder builder)
            where TContext : DbContext
        {
            Guard.AgainstNull(nameof(builder), builder);
            return Build(builder, x => (TContext) Activator.CreateInstance(typeof(TContext), x));
        }

        public static TContext Build<TContext>(string databaseName, bool enableSensitiveDataLogging = true)
            where TContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TContext>();
            builder.EnableSensitiveDataLogging(enableSensitiveDataLogging);
            return Build(builder, x => (TContext) Activator.CreateInstance(typeof(TContext), x), databaseName, DatabaseReusability.Active);
        }

        public static TContext Build<TContext>(DbContextOptionsBuilder builder, Func<DbContextOptions, TContext> contextConstructor)
            where TContext : DbContext
        {
            return Build(builder, contextConstructor, Guid.NewGuid().ToString());
        }

        public static TContext Build<TContext>(
            DbContextOptionsBuilder builder,
            Func<DbContextOptions, TContext> contextConstructor,
            string databaseName,
            DatabaseReusability reuseOption = DatabaseReusability.Disabled
        )
            where TContext : DbContext
        {
            Guard.AgainstNull(nameof(builder), builder);
            Guard.AgainstNull(nameof(contextConstructor), contextConstructor);
            builder.UseInMemoryDatabase(databaseName);
            builder.ReplaceService<IDbContextDependencies, DbContextDependenciesEx>();
            var context = contextConstructor(builder.Options);
            var exists = existingDatabases.Contains(databaseName);
            if (reuseOption != DatabaseReusability.Disabled && exists)
            {
                return context;
            }

            TrackDatabase(reuseOption, exists, databaseName);
            return context;
        }

        private static void TrackDatabase(DatabaseReusability reuseOption, bool exists, string databaseName)
        {
            if (reuseOption == DatabaseReusability.Active && !exists)
            {
                existingDatabases.Add(databaseName);
            }
        }
    }
}