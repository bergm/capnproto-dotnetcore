# capnproto-dotnetcore [![Build status](https://ci.appveyor.com/api/projects/status/tx4jjl2etiqve2xg/branch/master?svg=true)](https://ci.appveyor.com/project/c80k/capnproto-dotnetcore/branch/master) [![Coverage Status](https://coveralls.io/repos/github/c80k/capnproto-dotnetcore/badge.svg)](https://coveralls.io/github/c80k/capnproto-dotnetcore)

A Cap'n Proto implementation for .NET 10, written in C# 14. Credits to [lostinplace](https://github.com/lostinplace) and the other contributors.

["Cap'n Proto is an insanely fast data interchange format and capability-based RPC system."](https://capnproto.org/) Whilst the original implementation is written in C++ there are several ports to other languages. This is a C# implementation for .NET 10.

Disclaimer: Neither this project nor its author are affiliated with Cap'n Proto. This is just yet another independent implementation of the specification. The following sections assume that you are familiar with [Cap'n Proto](https://capnproto.org/) and probably its [GitHub project](https://github.com/capnproto/capnproto).

## Getting started: Users

The overall deployment consists of two components:
- The C# code generator back end is required for generating `.cs` serialization classes from `.capnp` schema files. It is designed to be used in conjunction with the Cap'n Proto tool set which is maintained at the original site. The tool set is required at compile time.
- The `Capnp.Net.Runtime` assembly is to be included as a reference into your particular application (or assembly).

### Code generator back end: Visual Studio / MSBuild integration

This is probably the most convenient option for Visual Studio development: The MSBuild integration package recognizes `.capnp` files in your VS project and generates their code-behind during build.

A prerequisite is that the Cap'n Proto tool suite is installed (`capnp.exe` must be on your `PATH`). The simplest way to achieve this:
```
choco install capnproto
```

Then, for the VS project which hosts your `.capnp` schema definitions:

```
Install-Package CapnpC.CSharp.MsBuild.Generation
```

### Code generator back end: dotnet tool

The C# code generator back end is available as dotnet tool. The version built from this repository requires the .NET 10 runtime or SDK. This is the recommended variant. To install it globally, type

```
dotnet tool install capnpc-csharp --global
```

### Code generator back end: Windows command line

There is also a self-contained [Chocolatey](https://chocolatey.org/) deployment for Windows (x86). To install, type

```
choco install capnpc-csharp-win-x86
```

This variant will also download and install the [Cap'n Proto tool set Chocolatey package](https://www.chocolatey.org/packages/capnproto). Note that the author does not maintain this package and has no influence on its contents.

### Runtime assembly

The `Capnp.Net.Runtime` assembly is available as [Nuget package](https://www.nuget.org/packages?q=Capnp.Net.Runtime). E.g. within VS package manage console, type

```
Install-Package Capnp.Net.Runtime
```

## Getting started: Developers

Build with the .NET 10 SDK selected by `global.json` (currently 10.0.400). All managed projects target `net10.0` and use C# 14. The test projects use MSTest SDK 4.4.0.

For native interoperability tests, install a C++ toolchain and the Cap'n Proto tools and libraries. On Windows, use the Visual C++ workload and [vcpkg](https://github.com/microsoft/vcpkg):

```
vcpkg install capnproto
```

Solution/project structure is as follows:
- `Capnp.Net.slnx` contains these projects:
  * `Capnp.Net.Runtime` is the runtime implementation for .NET 10.
  * `CapnpC.CSharp.Generator` contains the C# generator backend logic and targets .NET 10.
  * `capnpc-csharp` is the command-line generator backend, targeting .NET 10.
  * `CapnpC.CSharp.MsBuild.Generation` provides the MSBuild integration for the generator backend.
  * `Capnp.Net.Runtime.Tests` is an MS test assembly, containing - you guessed it - the test suite.
  * `CapnpC.CSharp.Generator.Tests` contains the generator backend test suite.
  * `CapnpC.CSharp.MsBuild.Generation.Tests` contains tests for `CapnpC.CSharp.MsBuild.Generation`.
- `CapnpCompatTest.slnx` builds the native interoperability executable which depends on the original Cap'n Proto C++ implementation. It is (partially) required by the test suite for interoperability testing.
- `MsBuildGenerationTest\MsBuildGenerationTest.slnx` tests MSBuild integration using the local runtime and generator projects by default. Set `PackageReferenceVersion` explicitly to test published packages instead.

## Features

The following Cap'n Proto features are currently implemented:
- Serialization/deserialization of all kinds of data (structs, groups, unions, lists, capabilities, data, text, enums, even primitives)
- Generics
- Level 1 RPC, including promise pipelining, embargos, and automatic tail calls
- Security (pointer validation, protection against amplification and stack overflow DoS attacks)
- Compiler backend generates reader/writer classes, interfaces, proxies, skeletons (as you know it from the C++ implementation), and additionally so-called "domain classes" for all struct types. A domain class is like a "plain old C# class" for representing a schema-defined struct, but it is decoupled from any underlying message. It provides serialize/deserialize methods for assembling/disassembling the actual message. This provides more convenience, but comes at the price of non-zero serialization overhead (not "infinitely" faster anymore).

These features are not yet implemented:
- Level N RPC with N ≥ 2
- Packing
- Compression
- Canonicalization
- Dynamic Reflection
- mmap
