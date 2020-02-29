using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

//TODO: remove when this is fixed https://github.com/aspnet/EntityFrameworkCore/issues/2166
namespace EfCore.InMemoryHelpers
{
    internal static class IndexValidator
    {
        public static void ValidateIndexes(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries().GroupBy(x => x.Metadata))
            {
                foreach (var index in entry.Key.UniqueIndices())
                {
                    var changeTrackingEntities = entry.Select(x => x.Entity);
                    var dbEntities = context.Query(entry.Key.ClrType).Cast<object>().Where(it => !changeTrackingEntities.Contains(it));

                    index.ValidateEntities(dbEntities.Union(changeTrackingEntities));
                }
            }
        }

        private static void ValidateEntities(this IIndex index, IEnumerable<object> entities)
        {
            var dictionary = new Dictionary<long, List<object>>();
            foreach (var entity in entities)
            {
                var valueLookup = index.GetProperties(entity).ToList();
                var values = valueLookup.Select(x => x.value).ToList();
                if (values.Any(x => x == null))
                {
                    continue;
                }

                var hash = values.GetHash();

                if (!dictionary.ContainsKey(hash))
                {
                    dictionary[hash] = values;
                    continue;
                }

                var builder = new StringBuilder($"Conflicting values for unique index. Entity: {entity.GetType().FullName},\r\nIndex Properties:\r\n");
                foreach ((var name, var value) in valueLookup)
                {
                    builder.AppendLine($"    {name}='{value}'");
                }

                throw new Exception(builder.ToString());
            }
        }

        private static IEnumerable<IIndex> UniqueIndices(this IEntityType entityType)
        {
            return entityType.GetIndexes()
                .Where(x => x.IsUnique);
        }

        private static long GetHash(this IEnumerable<object> values)
        {
            return string.Join("/", values).GetHashCode();
        }

        private static IEnumerable<(string name, object value)> GetProperties(this IIndex index, object entity)
        {
            return index.Properties
                .Select(property => property.PropertyInfo)
                .Select(info => (info.Name, info.GetValue(entity)));
        }
    }
}