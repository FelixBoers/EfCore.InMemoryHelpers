using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace EfCore.InMemoryHelpers
{
    public static class InMemoryContextBuilder
    {
        public static TContext Build<TContext>()
            where TContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TContext>();
            return Build<TContext>(builder);
        }

        public static TContext Build<TContext>(DbContextOptionsBuilder builder)
            where TContext : DbContext
        {
            Guard.AgainstNull(nameof(builder), builder);
            return Build(builder, x => (TContext) Activator.CreateInstance(typeof(TContext), x));
        }

        public static TContext Build<TContext>(DbContextOptionsBuilder builder, Func<DbContextOptions, TContext> contextConstructor)
            where TContext : DbContext
        {
            Guard.AgainstNull(nameof(builder), builder);
            Guard.AgainstNull(nameof(contextConstructor), contextConstructor);
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            builder.ReplaceService<IDbContextDependencies, DbContextDependenciesEx>();
            var context = contextConstructor(builder.Options);
            context.ResetValueGenerators();
            return context;
        }
    }
}