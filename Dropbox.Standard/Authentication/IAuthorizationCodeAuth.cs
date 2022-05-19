// <copyright file="IAuthorizationCodeAuthCredentials.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace Dropbox.Standard.Authentication
{
    using System;

    public interface IAuthorizationCodeAuth
    {
        /// <summary>
        /// Gets oAuthClientId.
        /// </summary>
        string OAuthClientId { get; }

        /// <summary>
        /// Gets oAuthClientSecret.
        /// </summary>
        string OAuthClientSecret { get; }

        /// <summary>
        /// Gets oAuthRedirectUri.
        /// </summary>
        string OAuthRedirectUri { get; }

        /// <summary>
        /// Gets oAuthToken.
        /// </summary>
        Models.OAuthToken OAuthToken { get; }

        /// <summary>
        ///  Returns true if credentials matched.
        /// </summary>
        bool Equals(string oAuthClientId, string oAuthClientSecret, string oAuthRedirectUri);
    }
}