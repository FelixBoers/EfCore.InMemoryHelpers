# EfCore.InMemoryHelpers

Provides a wrapper around the [EF Core In-Memory Database Provider](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/). Specifically works around the following EF bugs.

 * [InMemory: Improve in-memory key generation](https://github.com/aspnet/EntityFrameworkCore/issues/6872) <br>
  Reasoning: For many bootstrapping and integration tests, the id generation should be predictable, i.e. an in-memory persistence should not share static mutable state. This is especially important when using unit tests as a bootstrap to generate ad-hoc data. 
 * [Add foreign key constraint checking to the in-memory provider](https://github.com/aspnet/EntityFrameworkCore/issues/2166) <br>
  Reasoning: It is desirable for foreign keys to be validated when running unit tests. This allows bugs to be caught earlier without the need for integration testing against a real database.    


## NuGet  [![NuGet Status](http://img.shields.io/nuget/v/EfCore.InMemoryHelpers.svg?style=flat)](https://www.nuget.org/packages/EfCore.InMemoryHelpers/)

https://nuget.org/packages/EfCore.InMemoryHelpers/

    PM> Install-Package EfCore.InMemoryHelpers


## Usage

The main entry point is `InMemoryContextBuilder` which can be used to build an in-memory context.

```csharp
using (var context = InMemoryContextBuilder.Build<MyDataContext>())
{
    var entity = new MyEntity
    {
        Property = "prop"
    };
    context.Add(entity);
    context.SaveChanges();
}
```

A custom `DbContextOptionsBuilder` can be passed in:

```csharp
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
```

Both the above usages assume that the target context has a public constructor that accepts a `DbContextOptions`.

```csharp
public MyDataContext(DbContextOptions options) :
    base(options)
{
}
```

If this is not the case a custom context constructor can be passed in:

```csharp
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
```


## Icon

<a href="https://thenounproject.com/term/memory/884922/" target="_blank">memory</a> designed by Montu Yadav from [The Noun Project](https://thenounproject.com)