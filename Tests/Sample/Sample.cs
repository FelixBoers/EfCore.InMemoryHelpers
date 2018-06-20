using System;
using EfCore.InMemoryHelpers;
using Microsoft.EntityFrameworkCore;

class Sample
{
    void Simple()
    {
        using (var context = InMemoryContextBuilder.Build<MyDataContext>())
        {
            var entity = new MyEntity
            {
                Property = "prop"
            };
            context.Add(entity);
            context.SaveChanges();
        }
    }
    void WithBuilder()
    {
        var builder = new DbContextOptionsBuilder<MyDataContext>();
        using (var context = InMemoryContextBuilder.Build<MyDataContext>(builder))
        {
            var entity = new MyEntity
            {
                Property = "prop"
            };
            context.Add(entity);
            context.SaveChanges();
        }
    }
    void WithConstructor()
    {
        var builder = new DbContextOptionsBuilder<MyDataContext>();
        using (var context = InMemoryContextBuilder.Build(builder, options => new MyDataContext(options)))
        {
            var entity = new MyEntity
            {
                Property = "prop"
            };
            context.Add(entity);
            context.SaveChanges();
        }
    }
}