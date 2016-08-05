# ArksLayer.Tweaker.Abstractions
### A collection of interface and strongly-typed configuration classes for PSO2 Tweaker.

## IRenderer

Provides an interface that can be implemented for linking the UI against other `ArksLayer.Tweaker` modules. Current signatures:

```csharp
public interface IRenderer
{
    void WriteLine(string s);
    void AppendLog(string s);

	void OnPatchingStart(int fileCount);

    void OnDownloadStart(string url, WebClient client);
    void OnDownloadFinish(string url);
    void OnDownloadRetry(string url, int delaySecond);
    void OnDownloadAborted(string url);

    void OnHashStart();
    void OnHashProgress(int progress, int total);
    void OnHashComplete();
}
``` 

## ITweakerSettings

Provides an interface that can be loaded by other `ArksLayer.Tweaker` modules for strongly reading application settings. Current signatures:

```csharp
public interface ITweakerSettings
{
    string GameDirectory { set; get; }
}
```

## RegistryTweakerSettings

Implements `ITweakerSettings` against Windows Registry in `HKEY_CURRENT_USER`.

---

Project can be opened using Visual Studio 2015 Update 3.

Project can be packaged into NuGet package using .NET Core tooling: `dotnet pack -c Release`