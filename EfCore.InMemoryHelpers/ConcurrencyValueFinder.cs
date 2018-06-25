using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

static class ConcurrencyValueFinder
{
    public static bool GetConcurrency(this IEntityType entityType, out Action<object, byte[]> setter, out Func<object, byte[]> getter)
    {
        var concurrencyProperty = entityType.GetProperties().SingleOrDefault(x => x.IsConcurrencyToken);
        if (concurrencyProperty == null)
        {
            setter = null;
            getter = null;
            return false;
        }

        setter = (z, y) => concurrencyProperty.GetSetter().SetClrValue(z, y);
        getter = o => (byte[]) concurrencyProperty.GetGetter().GetClrValue(o);
        return true;
    }
}