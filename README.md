
# Getting Started with Dropbox

## Introduction

API for testing OAuth2 with Dropbox

## Building

The generated code uses the Newtonsoft Json.NET NuGet Package. If the automatic NuGet package restore is enabled, these dependencies will be installed automatically. Therefore, you will need internet access for build.

* Open the solution (Dropbox.sln) file.

Invoke the build process using Ctrl + Shift + B shortcut key or using the Build menu as shown below.

The build process generates a portable class library, which can be used like a normal class library. The generated library is compatible with Windows Forms, Windows RT, Windows Phone 8, Silverlight 5, Xamarin iOS, Xamarin Android and Mono. More information on how to use can be found at the MSDN Portable Class Libraries documentation.

## Installation

The following section explains how to use the Dropbox.Standard library in a new project.

### 1. Starting a new project

For starting a new project, right click on the current solution from the solution explorer and choose `Add -> New Project`.

![Add a new project in Visual Studio](https://apidocs.io/illustration/cs?workspaceFolder=Dropbox-CSharp&workspaceName=Dropbox&projectName=Dropbox.Standard&rootNamespace=Dropbox.Standard&step=addProject)

Next, choose `Console Application`, provide `TestConsoleProject` as the project name and click OK.

![Create a new Console Application in Visual Studio](https://apidocs.io/illustration/cs?workspaceFolder=Dropbox-CSharp&workspaceName=Dropbox&projectName=Dropbox.Standard&rootNamespace=Dropbox.Standard&step=createProject)

### 2. Set as startup project

The new console project is the entry point for the eventual execution. This requires us to set the `TestConsoleProject` as the start-up project. To do this, right-click on the `TestConsoleProject` and choose `Set as StartUp Project` form the context menu.

![Adding a project reference](https://apidocs.io/illustration/cs?workspaceFolder=Dropbox-CSharp&workspaceName=Dropbox&projectName=Dropbox.Standard&rootNamespace=Dropbox.Standard&step=setStartup)

### 3. Add reference of the library project

In order to use the Tester library in the new project, first we must add a project reference to the `TestConsoleProject`. First, right click on the `References` node in the solution explorer and click `Add Reference...`

![Adding a project reference](https://apidocs.io/illustration/cs?workspaceFolder=Dropbox-CSharp&workspaceName=Dropbox&projectName=Dropbox.Standard&rootNamespace=Dropbox.Standard&step=addReference)

Next, a window will be displayed where we must set the `checkbox` on `Tester.Tests` and click `OK`. By doing this, we have added a reference of the `Tester.Tests` project into the new `TestConsoleProject`.

![Creating a project reference](https://apidocs.io/illustration/cs?workspaceFolder=Dropbox-CSharp&workspaceName=Dropbox&projectName=Dropbox.Standard&rootNamespace=Dropbox.Standard&step=createReference)

### 4. Write sample code

Once the `TestConsoleProject` is created, a file named `Program.cs` will be visible in the solution explorer with an empty `Main` method. This is the entry point for the execution of the entire solution. Here, you can add code to initialize the client library and acquire the instance of a Controller class. Sample code to initialize the client library and using Controller methods is given in the subsequent sections.

![Adding a project reference](https://apidocs.io/illustration/cs?workspaceFolder=Dropbox-CSharp&workspaceName=Dropbox&projectName=Dropbox.Standard&rootNamespace=Dropbox.Standard&step=addCode)

## Initialize the API Client

**_Note:_** Documentation for the client can be found [here.](doc/client.md)

The following parameters are configurable for the API Client:

| Parameter | Type | Description |
|  --- | --- | --- |
| `Basepath` | `string` | Base path of the Dropbox API server<br>*Default*: `"api.dropboxapi.com"` |
| `Environment` | Environment | The API environment. <br> **Default: `Environment.Production`** |
| `Timeout` | `TimeSpan` | Http client timeout.<br>*Default*: `TimeSpan.FromSeconds(100)` |
| `OAuthClientId` | `string` | OAuth 2 Client ID |
| `OAuthClientSecret` | `string` | OAuth 2 Client Secret |
| `OAuthRedirectUri` | `string` | OAuth 2 Redirection endpoint or Callback Uri |
| `OAuthToken` | `Models.OAuthToken` | Object for storing information about the OAuth token |

The API client can be initialized as follows:

```csharp
Dropbox.Standard.DropboxClient client = new Dropbox.Standard.DropboxClient.Builder()
    .AuthorizationCodeAuth("OAuthClientId", "OAuthClientSecret", "OAuthRedirectUri")
    .Environment(Dropbox.Standard.Environment.Production)
    .Basepath("api.dropboxapi.com")
    .HttpClientConfig(config => config.NumberOfRetries(0))
    .Build();
```

## Authorization

This API uses `OAuth 2 Authorization Code Grant`.

## Authorization Code Grant

Your application must obtain user authorization before it can execute an endpoint call incase this SDK chooses to use *OAuth 2.0 Authorization Code Grant*. This authorization includes the following steps

### 1. Obtain user consent

To obtain user's consent, you must redirect the user to the authorization page.The `BuildAuthorizationUrl()` method creates the URL to the authorization page.

```csharp
string authUrl = client.Auth.BuildAuthorizationUrl();
```

### 2. Handle the OAuth server response

Once the user responds to the consent request, the OAuth 2.0 server responds to your application's access request by redirecting the user to the redirect URI specified set in `Configuration`.

If the user approves the request, the authorization code will be sent as the `code` query string:

```
https://example.com/oauth/callback?code=XXXXXXXXXXXXXXXXXXXXXXXXX
```

If the user does not approve the request, the response contains an `error` query string:

```
https://example.com/oauth/callback?error=access_denied
```

### 3. Authorize the client using the code

After the server receives the code, it can exchange this for an *access token*. The access token is an object containing information for authorizing client requests and refreshing the token itself.

```csharp
var authManager = client.AuthorizationCodeAuth as AuthorizationCodeAuthManager;

try
{
    OAuthToken token = authManager.Authorize();
}
catch (OAuthProviderException e)
{
    // TODO Handle exception
}
```

### Refreshing the token

An access token may expire after sometime. To extend its lifetime, you must refresh the token.

```csharp
if (authManager.IsTokenExpired())
{
    try
    {
        authManager.RefreshToken();
    }
catch (OAuthProviderException e)
{
    // TODO Handle exception
}
```

If a token expires, an exception will be thrown before the next endpoint call requiring authentication.

### Storing an access token for reuse

It is recommended that you store the access token for reuse.

```csharp
// store token
SaveTokenToDatabase(client.AuthorizationCodeAuth.OAuthToken);
```

### Creating a client from a stored token

To authorize a client from a stored access token, just set the access token in Configuration along with the other configuration parameters before creating the client:

```csharp
// load token later
OAuthToken token = LoadTokenFromDatabase();

// Provide token along with other configuration properties while instantiating the client
DropboxClient client = new DropboxClient.Builder()
    .OAuthToken(token)
    .Build();
```

## List of APIs

* [Files](doc/controllers/files.md)

## Classes Documentation

* [Utility Classes](doc/utility-classes.md)
* [HttpRequest](doc/http-request.md)
* [HttpResponse](doc/http-response.md)
* [HttpStringResponse](doc/http-string-response.md)
* [HttpContext](doc/http-context.md)
* [HttpClientConfiguration](doc/http-client-configuration.md)
* [HttpClientConfiguration Builder](doc/http-client-configuration-builder.md)
* [IAuthManager](doc/i-auth-manager.md)
* [ApiException](doc/api-exception.md)

