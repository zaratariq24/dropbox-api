
# O Auth Provider Exception

OAuth 2 Authorization endpoint exception.

## Structure

`OAuthProviderException`

## Fields

| Name | Type | Tags | Description |
|  --- | --- | --- | --- |
| `Error` | [`Models.OAuthProviderErrorEnum`](../../doc/models/o-auth-provider-error-enum.md) | Required | Gets or sets error code. |
| `ErrorDescription` | `string` | Optional | Gets or sets human-readable text providing additional information on error.<br>Used to assist the client developer in understanding the error that occurred. |
| `ErrorUri` | `string` | Optional | Gets or sets a URI identifying a human-readable web page with information about the error, used to provide the client developer with additional information about the error. |

## Example (as JSON)

```json
{
  "error": "invalid_request",
  "error_description": null,
  "error_uri": null
}
```

