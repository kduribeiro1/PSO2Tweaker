# ArksLayer.Tweaker.UpdateEngine
### A high performance game client update module for PSO2 Tweaker.

Simplest usage example:

```csharp
var output = new MyUiRenderer();
var settings = new RegistryTweakerSettings();
var updater = new UpdateManager(settings, output);

await updater.Update();
```

Also this:

```
updater.RestoreBackupFiles();
```

---

Project can be opened using Visual Studio 2015 Update 3.

Project can be packaged into NuGet package using .NET Core tooling: `dotnet pack -c Release`