// <copyright file="IConfiguration.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace Dropbox.Standard
{
    using System;
    using System.Net;
    using Dropbox.Standard.Authentication;
    using Dropbox.Standard.Models;

    /// <summary>
    /// IConfiguration.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets Current API environment.
        /// </summary>
        Environment Environment { get; }

        /// <summary>
        /// Gets Base path of the Dropbox API server
        /// </summary>
        string Basepath { get; }

        /// <summary>
        /// Gets the credentials to use with AuthorizationCodeAuth.
        /// </summary>
        IAuthorizationCodeAuth AuthorizationCodeAuth { get; }

        /// <summary>
        /// Gets the URL for a particular alias in the current environment and appends it with template parameters.
        /// </summary>
        /// <param name="alias">Default value:DEFAULT.</param>
        /// <returns>Returns the baseurl.</returns>
        string GetBaseUri(Server alias = Server.Default);
    }
}