// <copyright file="DropboxClient.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace Dropbox.Standard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Dropbox.Standard.Authentication;
    using Dropbox.Standard.Controllers;
    using Dropbox.Standard.Http.Client;
    using Dropbox.Standard.Utilities;

    /// <summary>
    /// The gateway for the SDK. This class acts as a factory for Controller and
    /// holds the configuration of the SDK.
    /// </summary>
    public sealed class DropboxClient : IConfiguration
    {
        // A map of environments and their corresponding servers/baseurls
        private static readonly Dictionary<Environment, Dictionary<Server, string>> EnvironmentsMap =
            new Dictionary<Environment, Dictionary<Server, string>>
        {
            {
                Environment.Production, new Dictionary<Server, string>
                {
                    { Server.Default, "https://{basepath}/2" },
                    { Server.TokenAuth, "https://api.dropbox.com/oauth2" },
                    { Server.Auth, "https://www.dropbox.com/oauth2" },
                }
            },
        };

        private readonly IDictionary<string, IAuthManager> authManagers;
        private readonly IHttpClient httpClient;
        private readonly AuthorizationCodeAuthManager authorizationCodeAuthManager;

        private readonly Lazy<FilesController> files;
        private readonly Lazy<OAuthAuthorizationController> oAuthAuthorization;

        private DropboxClient(
            Environment environment,
            string basepath,
            string oAuthClientId,
            string oAuthClientSecret,
            string oAuthRedirectUri,
            Models.OAuthToken oAuthToken,
            IDictionary<string, IAuthManager> authManagers,
            IHttpClient httpClient,
            IHttpClientConfiguration httpClientConfiguration)
        {
            this.Environment = environment;
            this.Basepath = basepath;
            this.httpClient = httpClient;
            this.authManagers = (authManagers == null) ? new Dictionary<string, IAuthManager>() : new Dictionary<string, IAuthManager>(authManagers);
            this.HttpClientConfiguration = httpClientConfiguration;

            this.files = new Lazy<FilesController>(
                () => new FilesController(this, this.httpClient, this.authManagers));
            this.oAuthAuthorization = new Lazy<OAuthAuthorizationController>(
                () => new OAuthAuthorizationController(this, this.httpClient, this.authManagers));

            if (this.authManagers.ContainsKey("global"))
            {
                this.authorizationCodeAuthManager = (AuthorizationCodeAuthManager)this.authManagers["global"];
            }

            if (!this.authManagers.ContainsKey("global")
                || !this.AuthorizationCodeAuth.Equals(oAuthClientId, oAuthClientSecret, oAuthRedirectUri))
            {
                this.authorizationCodeAuthManager = new AuthorizationCodeAuthManager(oAuthClientId, oAuthClientSecret, oAuthRedirectUri, oAuthToken, this);
                this.authManagers["global"] = this.authorizationCodeAuthManager;
            }
        }

        /// <summary>
        /// Gets FilesController controller.
        /// </summary>
        public FilesController FilesController => this.files.Value;

        /// <summary>
        /// Gets OAuthAuthorizationController controller.
        /// </summary>
        public OAuthAuthorizationController OAuthAuthorizationController => this.oAuthAuthorization.Value;

        /// <summary>
        /// Gets the configuration of the Http Client associated with this client.
        /// </summary>
        public IHttpClientConfiguration HttpClientConfiguration { get; }

        /// <summary>
        /// Gets Environment.
        /// Current API environment.
        /// </summary>
        public Environment Environment { get; }

        /// <summary>
        /// Gets Basepath.
        /// Base path of the Dropbox API server.
        /// </summary>
        public string Basepath { get; }

        /// <summary>
        /// Gets auth managers.
        /// </summary>
        internal IDictionary<string, IAuthManager> AuthManagers => this.authManagers;

        /// <summary>
        /// Gets http client.
        /// </summary>
        internal IHttpClient HttpClient => this.httpClient;

        /// <summary>
        /// Gets the credentials to use with AuthorizationCodeAuth.
        /// </summary>
        public IAuthorizationCodeAuth AuthorizationCodeAuth => this.authorizationCodeAuthManager;

        /// <summary>
        /// Gets the URL for a particular alias in the current environment and appends
        /// it with template parameters.
        /// </summary>
        /// <param name="alias">Default value:DEFAULT.</param>
        /// <returns>Returns the baseurl.</returns>
        public string GetBaseUri(Server alias = Server.Default)
        {
            StringBuilder url = new StringBuilder(EnvironmentsMap[this.Environment][alias]);
            ApiHelper.AppendUrlWithTemplateParameters(url, this.GetBaseUriParameters());

            return url.ToString();
        }

        /// <summary>
        /// Creates an object of the DropboxClient using the values provided for the builder.
        /// </summary>
        /// <returns>Builder.</returns>
        public Builder ToBuilder()
        {
            Builder builder = new Builder()
                .Environment(this.Environment)
                .Basepath(this.Basepath)
                .OAuthToken(this.authorizationCodeAuthManager.OAuthToken)
                .AuthorizationCodeAuth(this.authorizationCodeAuthManager.OAuthClientId, this.authorizationCodeAuthManager.OAuthClientSecret, this.authorizationCodeAuthManager.OAuthRedirectUri)
                .HttpClient(this.httpClient)
                .AuthManagers(this.authManagers)
                .HttpClientConfig(config => config.Build());

            return builder;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return
                $"Environment = {this.Environment}, " +
                $"Basepath = {this.Basepath}, " +
                $"HttpClientConfiguration = {this.HttpClientConfiguration}, ";
        }

        /// <summary>
        /// Creates the client using builder.
        /// </summary>
        /// <returns> DropboxClient.</returns>
        internal static DropboxClient CreateFromEnvironment()
        {
            var builder = new Builder();

            string environment = System.Environment.GetEnvironmentVariable("DROPBOX_STANDARD_ENVIRONMENT");
            string basepath = System.Environment.GetEnvironmentVariable("DROPBOX_STANDARD_BASEPATH");
            string oAuthClientId = System.Environment.GetEnvironmentVariable("DROPBOX_STANDARD_O_AUTH_CLIENT_ID");
            string oAuthClientSecret = System.Environment.GetEnvironmentVariable("DROPBOX_STANDARD_O_AUTH_CLIENT_SECRET");
            string oAuthRedirectUri = System.Environment.GetEnvironmentVariable("DROPBOX_STANDARD_O_AUTH_REDIRECT_URI");

            if (environment != null)
            {
                builder.Environment(ApiHelper.JsonDeserialize<Environment>($"\"{environment}\""));
            }

            if (basepath != null)
            {
                builder.Basepath(basepath);
            }

            if (oAuthClientId != null && oAuthClientSecret != null && oAuthRedirectUri != null)
            {
                builder.AuthorizationCodeAuth(oAuthClientId, oAuthClientSecret, oAuthRedirectUri);
            }

            return builder.Build();
        }

        /// <summary>
        /// Makes a list of the BaseURL parameters.
        /// </summary>
        /// <returns>Returns the parameters list.</returns>
        private List<KeyValuePair<string, object>> GetBaseUriParameters()
        {
            List<KeyValuePair<string, object>> kvpList = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("basepath", this.Basepath),
            };
            return kvpList;
        }

        /// <summary>
        /// Builder class.
        /// </summary>
        public class Builder
        {
            private Environment environment = Dropbox.Standard.Environment.Production;
            private string basepath = "api.dropboxapi.com";
            private string oAuthClientId = "";
            private string oAuthClientSecret = "";
            private string oAuthRedirectUri = "";
            private Models.OAuthToken oAuthToken = null;
            private IDictionary<string, IAuthManager> authManagers = new Dictionary<string, IAuthManager>();
            private HttpClientConfiguration.Builder httpClientConfig = new HttpClientConfiguration.Builder();
            private IHttpClient httpClient;

            /// <summary>
            /// Sets credentials for AuthorizationCodeAuth.
            /// </summary>
            /// <param name="oAuthClientId">OAuthClientId.</param>
            /// <param name="oAuthClientSecret">OAuthClientSecret.</param>
            /// <param name="oAuthRedirectUri">OAuthRedirectUri.</param>
            /// <returns>Builder.</returns>
            public Builder AuthorizationCodeAuth(string oAuthClientId, string oAuthClientSecret, string oAuthRedirectUri)
            {
                this.oAuthClientId = oAuthClientId ?? throw new ArgumentNullException(nameof(oAuthClientId));
                this.oAuthClientSecret = oAuthClientSecret ?? throw new ArgumentNullException(nameof(oAuthClientSecret));
                this.oAuthRedirectUri = oAuthRedirectUri ?? throw new ArgumentNullException(nameof(oAuthRedirectUri));
                return this;
            }

            /// <summary>
            /// Sets OAuthToken.
            /// </summary>
            /// <param name="oAuthToken">OAuthToken.</param>
            /// <returns>Builder.</returns>
            public Builder OAuthToken(Models.OAuthToken oAuthToken)
            {
                this.oAuthToken = oAuthToken;
                return this;
            }

            /// <summary>
            /// Sets Environment.
            /// </summary>
            /// <param name="environment"> Environment. </param>
            /// <returns> Builder. </returns>
            public Builder Environment(Environment environment)
            {
                this.environment = environment;
                return this;
            }

            /// <summary>
            /// Sets Basepath.
            /// </summary>
            /// <param name="basepath"> Basepath. </param>
            /// <returns> Builder. </returns>
            public Builder Basepath(string basepath)
            {
                this.basepath = basepath ?? throw new ArgumentNullException(nameof(basepath));
                return this;
            }

            /// <summary>
            /// Sets HttpClientConfig.
            /// </summary>
            /// <param name="action"> Action. </param>
            /// <returns>Builder.</returns>
            public Builder HttpClientConfig(Action<HttpClientConfiguration.Builder> action)
            {
                if (action is null)
                {
                    throw new ArgumentNullException(nameof(action));
                }

                action(this.httpClientConfig);
                return this;
            }

            /// <summary>
            /// Sets the IHttpClient for the Builder.
            /// </summary>
            /// <param name="httpClient"> http client. </param>
            /// <returns>Builder.</returns>
            internal Builder HttpClient(IHttpClient httpClient)
            {
                this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
                return this;
            }

            /// <summary>
            /// Sets the authentication managers for the Builder.
            /// </summary>
            /// <param name="authManagers"> auth managers. </param>
            /// <returns>Builder.</returns>
            internal Builder AuthManagers(IDictionary<string, IAuthManager> authManagers)
            {
                this.authManagers = authManagers ?? throw new ArgumentNullException(nameof(authManagers));
                return this;
            }

            /// <summary>
            /// Creates an object of the DropboxClient using the values provided for the builder.
            /// </summary>
            /// <returns>DropboxClient.</returns>
            public DropboxClient Build()
            {
                this.httpClient = new HttpClientWrapper(this.httpClientConfig.Build());

                return new DropboxClient(
                    this.environment,
                    this.basepath,
                    this.oAuthClientId,
                    this.oAuthClientSecret,
                    this.oAuthRedirectUri,
                    this.oAuthToken,
                    this.authManagers,
                    this.httpClient,
                    this.httpClientConfig.Build());
            }
        }
    }
}
