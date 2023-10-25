// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace Laserfiche.Api.Client.Utils
{
    public static class DomainUtils
    {
        /// <summary>
        /// Returns the Laserfiche Cloud OAuth Api base uri using the given domain.
        /// </summary>
        /// <param name="domain">The Laserfiche domain.</param>
        /// <returns>The Laserfiche Cloud OAuth Api base uri.</returns>
        public static string GetOAuthApiBaseUri(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentNullException(nameof(domain));
            return $"https://signin.{domain}/oauth/";
        }

        /// <summary>
        /// Returns the Laserfiche Cloud Repository Api base uri using the given domain.
        /// </summary>
        /// <param name="domain">The Laserfiche domain.</param>
        /// <returns>The Laserfiche Cloud Repository Api base uri.</returns>
        public static string GetRepositoryApiBaseUri(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentNullException(nameof(domain));
            return $"https://api.{domain}/repository/";
        }
    }
}
