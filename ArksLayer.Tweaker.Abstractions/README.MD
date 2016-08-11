# ArksLayer.Tweaker.Abstractions
### A collection of interfaces and strongly-typed configuration classes for PSO2 Tweaker.

## ITrigger

Provides the definitions for an assortment of methods that can be implemented for linking a UI against other `ArksLayer.Tweaker` modules.

## ITweakerSettings

Provides property definitions that can be implemented for providing a `ArksLayer.Tweaker` with strongly-typed application settings.

## RegistryTweakerSettings

Implements `ITweakerSettings` against Windows Registry in `HKEY_CURRENT_USER`.

---

Project can be opened using Visual Studio 2015 Update 3.

Project can be packaged into NuGet package using .NET Core tooling: `dotnet pack -c Release`