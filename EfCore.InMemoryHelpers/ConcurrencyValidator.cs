using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

class ConcurrencyValidator
{
    List<object> seen = new List<object>();

    public void ValidateIndexes(DbContext context)
    {
        foreach (var grouping in context.ChangeTracker.Entries().GroupBy(x => x.Metadata))
        {
            var entityType = grouping.Key;
            if (!entityType.GetConcurrency(out var setter, out var getter))
            {
                continue;
            }

            var primaryKey = entityType.GetProperties().SingleOrDefault(x => x.IsPrimaryKey());
            if (primaryKey == null)
            {
                continue;
            }

            var entries = grouping.ToList();
            var objects = entries.Select(x => x.Entity).ToList();

            var primaryKeyGetter = primaryKey.GetGetter();
            foreach (var objectsByKey in objects.GroupBy(x => primaryKeyGetter.GetClrValue(x)))
            {
                Validate(getter, setter, primaryKey, objectsByKey.Key, objectsByKey.ToList());
            }
        }
    }

    void Validate(Func<object, byte[]> getter, Action<object, byte[]> setter, IProperty primaryKey, object primaryKeyValue, List<object> objects)
    {
        byte[] rowVersion;
        var first = objects.First();

        var exceptionSuffix = $" Type: {first.GetType().FullName}. {primaryKey.Name}: {primaryKeyValue}.";
        //If seen
        if (seen.Any(x => ReferenceEquals(x, first)))
        {
            rowVersion = getter(first);
            if (rowVersion == null)
            {
                throw new Exception($"Row version has been incorrectly set to null. {exceptionSuffix}");
            }
        }
        //If not seen
        else
        {
            if (getter(first) != null)
            {
                throw new Exception($"The first save must have a null RowVersion. {exceptionSuffix}");
            }

            rowVersion = RowVersion.New();
            setter(first, rowVersion);
            seen.Add(first);
        }

        foreach (var o in objects.Skip(1))
        {
            var bytes = getter(o);

            if (bytes !=null && bytes.SequenceEqual(rowVersion))
            {
                rowVersion = RowVersion.New();
                setter(o, rowVersion);
            }
        }
    }
}