// <copyright file="OAuthAuthorizationController.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace Dropbox.Standard.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Dropbox.Standard;
    using Dropbox.Standard.Authentication;
    using Dropbox.Standard.Exceptions;
    using Dropbox.Standard.Http.Client;
    using Dropbox.Standard.Http.Request;
    using Dropbox.Standard.Http.Request.Configuration;
    using Dropbox.Standard.Http.Response;
    using Dropbox.Standard.Utilities;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// OAuthAuthorizationController.
    /// </summary>
    public class OAuthAuthorizationController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthAuthorizationController"/> class.
        /// </summary>
        /// <param name="config"> config instance. </param>
        /// <param name="httpClient"> httpClient. </param>
        /// <param name="authManagers"> authManager. </param>
        internal OAuthAuthorizationController(IConfiguration config, IHttpClient httpClient, IDictionary<string, IAuthManager> authManagers)
            : base(config, httpClient, authManagers)
        {
        }

        /// <summary>
        /// Create a new OAuth 2 token.
        /// </summary>
        /// <param name="authorization">Required parameter: Authorization header in Basic auth format.</param>
        /// <param name="code">Required parameter: Authorization Code.</param>
        /// <param name="redirectUri">Required parameter: Redirect Uri.</param>
        /// <param name="fieldParameters">Additional optional form parameters are supported by this endpoint.</param>
        /// <returns>Returns the Models.OAuthToken response from the API call.</returns>
        public Models.OAuthToken RequestToken(
                string authorization,
                string code,
                string redirectUri,
                Dictionary<string, object> fieldParameters = null)
        {
            Task<Models.OAuthToken> t = this.RequestTokenAsync(authorization, code, redirectUri, fieldParameters);
            ApiHelper.RunTaskSynchronously(t);
            return t.Result;
        }

        /// <summary>
        /// Create a new OAuth 2 token.
        /// </summary>
        /// <param name="authorization">Required parameter: Authorization header in Basic auth format.</param>
        /// <param name="code">Required parameter: Authorization Code.</param>
        /// <param name="redirectUri">Required parameter: Redirect Uri.</param>
        /// <param name="fieldParameters">Additional optional form parameters are supported by this endpoint.</param>
        /// <param name="cancellationToken"> cancellationToken. </param>
        /// <returns>Returns the Models.OAuthToken response from the API call.</returns>
        public async Task<Models.OAuthToken> RequestTokenAsync(
                string authorization,
                string code,
                string redirectUri,
                Dictionary<string, object> fieldParameters = null,
                CancellationToken cancellationToken = default)
        {
            // the base uri for api requests.
            string baseUri = this.Config.GetBaseUri(Server.TokenAuth);

            // prepare query string for API call.
            StringBuilder queryBuilder = new StringBuilder(baseUri);
            queryBuilder.Append("/token");

            // append request with appropriate headers and parameters
            var headers = new Dictionary<string, string>()
            {
                { "user-agent", this.UserAgent },
                { "accept", "application/json" },
                { "Authorization", authorization },
            };

            // append form/field parameters.
            var fields = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("grant_type", "authorization_code"),
                new KeyValuePair<string, object>("code", code),
                new KeyValuePair<string, object>("redirect_uri", redirectUri),
            };

            // optional form parameters.
            fields.AddRange(ApiHelper.PrepareFormFieldsFromObject(string.Empty, fieldParameters, arrayDeserializationFormat: this.ArrayDeserializationFormat));

            // remove null parameters.
            fields = fields.Where(kvp => kvp.Value != null).ToList();

            // prepare the API call request to fetch the response.
            HttpRequest httpRequest = this.GetClientInstance().Post(queryBuilder.ToString(), headers, fields);

            // invoke request and get response.
            HttpStringResponse response = await this.GetClientInstance().ExecuteAsStringAsync(httpRequest, cancellationToken: cancellationToken).ConfigureAwait(false);
            HttpContext context = new HttpContext(httpRequest, response);

            if (response.StatusCode == 400)
            {
                throw new OAuthProviderException("OAuth 2 provider returned an error.", context);
            }

            if (response.StatusCode == 401)
            {
                throw new OAuthProviderException("OAuth 2 provider says client authentication failed.", context);
            }

            // handle errors defined at the API level.
            this.ValidateResponse(response, context);

            return ApiHelper.JsonDeserialize<Models.OAuthToken>(response.Body);
        }

        /// <summary>
        /// Obtain a new access token using a refresh token.
        /// </summary>
        /// <param name="authorization">Required parameter: Authorization header in Basic auth format.</param>
        /// <param name="refreshToken">Required parameter: Refresh token.</param>
        /// <param name="scope">Optional parameter: Requested scopes as a space-delimited list..</param>
        /// <param name="fieldParameters">Additional optional form parameters are supported by this endpoint.</param>
        /// <returns>Returns the Models.OAuthToken response from the API call.</returns>
        public Models.OAuthToken RefreshToken(
                string authorization,
                string refreshToken,
                string scope = null,
                Dictionary<string, object> fieldParameters = null)
        {
            Task<Models.OAuthToken> t = this.RefreshTokenAsync(authorization, refreshToken, scope, fieldParameters);
            ApiHelper.RunTaskSynchronously(t);
            return t.Result;
        }

        /// <summary>
        /// Obtain a new access token using a refresh token.
        /// </summary>
        /// <param name="authorization">Required parameter: Authorization header in Basic auth format.</param>
        /// <param name="refreshToken">Required parameter: Refresh token.</param>
        /// <param name="scope">Optional parameter: Requested scopes as a space-delimited list..</param>
        /// <param name="fieldParameters">Additional optional form parameters are supported by this endpoint.</param>
        /// <param name="cancellationToken"> cancellationToken. </param>
        /// <returns>Returns the Models.OAuthToken response from the API call.</returns>
        public async Task<Models.OAuthToken> RefreshTokenAsync(
                string authorization,
                string refreshToken,
                string scope = null,
                Dictionary<string, object> fieldParameters = null,
                CancellationToken cancellationToken = default)
        {
            // the base uri for api requests.
            string baseUri = this.Config.GetBaseUri(Server.TokenAuth);

            // prepare query string for API call.
            StringBuilder queryBuilder = new StringBuilder(baseUri);
            queryBuilder.Append("/token");

            // append request with appropriate headers and parameters
            var headers = new Dictionary<string, string>()
            {
                { "user-agent", this.UserAgent },
                { "accept", "application/json" },
                { "Authorization", authorization },
            };

            // append form/field parameters.
            var fields = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("grant_type", "refresh_token"),
                new KeyValuePair<string, object>("refresh_token", refreshToken),
                new KeyValuePair<string, object>("scope", scope),
            };

            // optional form parameters.
            fields.AddRange(ApiHelper.PrepareFormFieldsFromObject(string.Empty, fieldParameters, arrayDeserializationFormat: this.ArrayDeserializationFormat));

            // remove null parameters.
            fields = fields.Where(kvp => kvp.Value != null).ToList();

            // prepare the API call request to fetch the response.
            HttpRequest httpRequest = this.GetClientInstance().Post(queryBuilder.ToString(), headers, fields);

            // invoke request and get response.
            HttpStringResponse response = await this.GetClientInstance().ExecuteAsStringAsync(httpRequest, cancellationToken: cancellationToken).ConfigureAwait(false);
            HttpContext context = new HttpContext(httpRequest, response);

            if (response.StatusCode == 400)
            {
                throw new OAuthProviderException("OAuth 2 provider returned an error.", context);
            }

            if (response.StatusCode == 401)
            {
                throw new OAuthProviderException("OAuth 2 provider says client authentication failed.", context);
            }

            // handle errors defined at the API level.
            this.ValidateResponse(response, context);

            return ApiHelper.JsonDeserialize<Models.OAuthToken>(response.Body);
        }
    }
}