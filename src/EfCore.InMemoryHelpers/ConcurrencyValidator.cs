using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EfCore.InMemoryHelpers
{
    internal class ConcurrencyValidator
    {
        private readonly List<object> seen = new List<object>();

        public void ValidateIndexes(DbContext context)
        {
            foreach (var grouping in context.ChangeTracker.Entries().GroupBy(x => x.Metadata))
            {
                var entityType = grouping.Key;
                if (!entityType.GetConcurrency(out var setter, out var getter))
                {
                    continue;
                }

                var entries = grouping.ToList();
                var objects = entries.Select(x => x.Entity).ToList();
                Validate(getter, setter, objects);
            }
        }

        private void Validate(Func<object, byte[]> getter, Action<object, byte[]> setter, List<object> objects)
        {
            byte[] rowVersion;
            var first = objects.First();

            var version = getter(first);
            if (seen.Any(x => ReferenceEquals(x, first)))
            {
                rowVersion = version;
                if (rowVersion == null)
                {
                    throw new Exception("Row version has been incorrectly set to null");
                }
            }
            //If not seen
            else
            {
                if (version != null)
                {
                    throw new Exception("The first save must have a null RowVersion");
                }

                rowVersion = RowVersion.New();
                setter(first, rowVersion);
                seen.Add(first);
            }

            foreach (var o in objects.Skip(1))
            {
                var bytes = getter(o);

                if (bytes != null && bytes.SequenceEqual(rowVersion))
                {
                    rowVersion = RowVersion.New();
                    setter(o, rowVersion);
                }
            }
        }
    }
}