
# Client Class Documentation

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

## DropboxClient Class

The gateway for the SDK. This class acts as a factory for the Controllers and also holds the configuration of the SDK.

### Controllers

| Name | Description |
|  --- | --- |
| FilesController | Gets FilesController controller. |
| OAuthAuthorizationController | Gets OAuthAuthorizationController controller. |

### Properties

| Name | Description | Type |
|  --- | --- | --- |
| Auth | Gets the AuthManager. | `AuthManager` |
| HttpClientConfiguration | Gets the configuration of the Http Client associated with this client. | `IHttpClientConfiguration` |
| Timeout | Http client timeout. | `TimeSpan` |
| Environment | Current API environment. | `Environment` |
| Basepath | Base path of the Dropbox API server | `string` |

### Methods

| Name | Description | Return Type |
|  --- | --- | --- |
| `GetBaseUri(Server alias = Server.Default)` | Gets the URL for a particular alias in the current environment and appends it with template parameters. | `string` |
| `ToBuilder()` | Creates an object of the DropboxClient using the values provided for the builder. | `Builder` |

## DropboxClient Builder Class

Class to build instances of DropboxClient.

### Methods

| Name | Description | Return Type |
|  --- | --- | --- |
| `Auth(AuthManager auth)` | Gets the AuthManager. | `Builder` |
| `HttpClientConfiguration(Action<HttpClientConfiguration.Builder> action)` | Gets the configuration of the Http Client associated with this client. | `Builder` |
| `Timeout(TimeSpan timeout)` | Http client timeout. | `Builder` |
| `Environment(Environment environment)` | Current API environment. | `Builder` |
| `Basepath(string basepath)` | Base path of the Dropbox API server | `Builder` |

