# Files

```csharp
FilesController filesController = client.FilesController;
```

## Class Name

`FilesController`


# Get File Count

```csharp
GetFileCountAsync()
```

## Response Type

[`Task<Models.Response>`](../../doc/models/response.md)

## Example Usage

```csharp
try
{
    Response result = await filesController.GetFileCountAsync();
}
catch (ApiException e){};
```

