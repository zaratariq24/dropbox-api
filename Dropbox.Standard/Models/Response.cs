// <copyright file="Response.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace Dropbox.Standard.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Dropbox.Standard;
    using Dropbox.Standard.Utilities;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Response.
    /// </summary>
    public class Response : BaseModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        public Response()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="fileRequestCount">file_request_count.</param>
        public Response(
            int fileRequestCount)
        {
            this.FileRequestCount = fileRequestCount;
        }

        /// <summary>
        /// Gets or sets FileRequestCount.
        /// </summary>
        [JsonProperty("file_request_count")]
        public int FileRequestCount { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var toStringOutput = new List<string>();

            this.ToString(toStringOutput);

            return $"Response : ({string.Join(", ", toStringOutput)})";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj == this)
            {
                return true;
            }

            return obj is Response other &&
                this.FileRequestCount.Equals(other.FileRequestCount);
        }
        

        /// <summary>
        /// ToString overload.
        /// </summary>
        /// <param name="toStringOutput">List of strings.</param>
        protected new void ToString(List<string> toStringOutput)
        {
            toStringOutput.Add($"this.FileRequestCount = {this.FileRequestCount}");

            base.ToString(toStringOutput);
        }
    }
}