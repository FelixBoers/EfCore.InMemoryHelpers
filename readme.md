<!--
This file was generate by MarkdownSnippets.
Source File: /readme.source.md
To change this file edit the source file and then re-run the generation using either the dotnet global tool (https://github.com/SimonCropp/MarkdownSnippets#githubmarkdownsnippets) or using the api (https://github.com/SimonCropp/MarkdownSnippets#running-as-a-unit-test).
-->
# EfCore.InMemoryHelpers 

[![Actively Maintained](https://img.shields.io/badge/Maintenance%20Level-Actively%20Maintained-green.svg)](https://gist.github.com/cheerfulstoic/d107229326a01ff0f333a1d3476e068d)
[![Build status](https://ci.appveyor.com/api/projects/status/f1kk9pthruopfac9?svg=true)](https://ci.appveyor.com/project/flex87/efcore-inmemoryhelpers)
[![NuGet Status](http://img.shields.io/nuget/v/EfCore.InMemoryHelpers.svg?longCache=true&style=flat)](https://www.nuget.org/packages/EfCore.InMemoryHelpers/)

Provides a wrapper around the [EF Core In-Memory Database Provider](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/). Specifically works around the following EF bugs.

 * [InMemory: Improve in-memory key generation](https://github.com/aspnet/EntityFrameworkCore/issues/6872) <br>
  Reasoning: For many bootstrapping and integration tests, the id generation should be predictable, i.e. an in-memory persistence should not share static mutable state. This is especially important when using unit tests as a bootstrap to generate ad-hoc data.
 * Add index validation<br>
  Reasoning: It is desirable for indexes to be validated when running unit tests. This allows bugs to be caught earlier without the need for integration testing against a real database.
 * Add support for [Timestamp/row version](https://docs.microsoft.com/en-us/ef/core/modeling/concurrency#timestamprow-version) (`[Timestamp]`, and `.Property(p => p.X).IsRowVersion()`)<br>
  Reasoning: It is desirable for exceptions to be thrown when a update violates a RowVersion. This allows bugs to be caught earlier without the need for integration testing against a real database.

**This project is supported by the community via [Patreon sponsorship](https://www.patreon.com/join/simoncropp). If you are using this project to deliver business value or build commercial software it is expected that you will provide support [via Patreon](https://www.patreon.com/join/simoncropp).**


## NuGet 

https://nuget.org/packages/EfCore.InMemoryHelpers/

    PM> Install-Package EfCore.InMemoryHelpers


## Usage

The main entry point is `InMemoryContextBuilder` which can be used to build an in-memory context.

<!-- snippet: simple -->
```cs
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
<sup>[snippet source](/test/EfCore.InMemoryHelpers.Test/Snippets/Sample.cs#L9-L21)</sup>
<!-- endsnippet -->

A custom `DbContextOptionsBuilder` can be passed in:

<!-- snippet: withBuilder -->
```cs
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
<sup>[snippet source](/test/EfCore.InMemoryHelpers.Test/Snippets/Sample.cs#L26-L39)</sup>
<!-- endsnippet -->

Both the above usages assume that the target context has a public constructor that accepts a `DbContextOptions`.

<!-- snippet: dataContextCtor -->
```cs
public MyDataContext(DbContextOptions options)
    :
    base(options)
{ }
```
<sup>[snippet source](/test/EfCore.InMemoryHelpers.Test/Snippets/MyDataContext.cs#L7-L14)</sup>
<!-- endsnippet -->

If this is not the case a custom context constructor can be passed in:

<!-- snippet: customContextConstructor -->
```cs
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
<sup>[snippet source](/test/EfCore.InMemoryHelpers.Test/Snippets/Sample.cs#L44-L57)</sup>
<!-- endsnippet -->


## Icon

<a href="https://thenounproject.com/term/memory/884922/" target="_blank">memory</a> designed by Montu Yadav from [The Noun Project](https://thenounproject.com)
