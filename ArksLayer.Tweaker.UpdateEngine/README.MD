# ArksLayer.Tweaker.UpdateEngine
### A high performance game client update module for PSO2 Tweaker.

Simplest usage example:

```csharp
var output = new MyTriggers();
var settings = new RegistryTweakerSettings();
var updater = new UpdateManager(settings, output);

await updater.Update(false, true);
```

Also capable of:

- Checking whether the game client version is up to date.
- Restoring all backup files, indiscriminately.
- Cleaning files that are not supposed to exist before the next update and removing in-game chat censor.

---

Project can be opened using Visual Studio 2015 Update 3.

Project can be packaged into NuGet package using .NET Core tooling: `dotnet pack -c Release`