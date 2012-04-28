# Facebook Sample using Facebook C# SDK with Windows Phone 7

This sample demonstrates the use of Facebook C# SDK v6 as a Windows Phone 7 Application.

_Note: This sample does not necessarily demonstrate the best use but rather features of using Facebook C# SDK on a Windows Phone 7 app. Always remember to handle exceptions_

# Getting started

Set the appropriate `AppId` in `FacebookLoginPage.xaml.cs` before running the sample.

```csharp
private const string AppId = "app_id";
```


_**Note:**
For new projects using Facebook C# SDK which use anonymous objects as parameters make sure to add `[assembly: InternalsVisibleTo("Facebook")]` in AssemblyInfo.cs._