using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace EfCore.InMemoryHelpers
{
    internal static class RowVersion
    {
        private static long counter;

        public static Guid GetGuid(this byte[] rowVersion)
        {
            return new Guid(rowVersion);
        }

        public static string GetString(this byte[] rowVersion)
        {
            return new Guid(rowVersion).ToString();
        }

        public static byte[] New()
        {
            return Guid.NewGuid().ToByteArray();
        }

        public static ulong NewLong()
        {
            return (ulong)Interlocked.Increment(ref counter);
        }

        public static IQueryable Query(this DbContext context, string entityName) =>
            context.Query(context.Model.FindEntityType(entityName).ClrType);

        static readonly MethodInfo SetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set));

        public static IQueryable Query(this DbContext context, Type entityType) =>
            (IQueryable)SetMethod.MakeGenericMethod(entityType).Invoke(context, null);
    }
}