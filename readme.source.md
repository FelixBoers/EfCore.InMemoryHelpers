# EfCore.InMemoryHelpers 

[![Actively Maintained](https://img.shields.io/badge/Maintenance%20Level-Actively%20Maintained-green.svg)](https://gist.github.com/cheerfulstoic/d107229326a01ff0f333a1d3476e068d)
[![Build status](https://ci.appveyor.com/api/projects/status/f1kk9pthruopfac9?svg=true)](https://ci.appveyor.com/project/flex87/efcore-inmemoryhelpers)
[![NuGet Status](http://img.shields.io/nuget/v/EfCore.InMemoryHelpers.svg?longCache=true&style=flat)](https://www.nuget.org/packages/EfCore.InMemoryHelpers/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/FelixBoers/EfCore.InMemoryHelpers/blob/master/license.txt)

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

snippet: simple

A custom `DbContextOptionsBuilder` can be passed in:

snippet: withBuilder

Both the above usages assume that the target context has a public constructor that accepts a `DbContextOptions`.

snippet: dataContextCtor

If this is not the case a custom context constructor can be passed in:

snippet: customContextConstructor


## Icon

<a href="https://thenounproject.com/term/memory/884922/" target="_blank">memory</a> designed by Montu Yadav from [The Noun Project](https://thenounproject.com)
