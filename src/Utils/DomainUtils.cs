using System;

namespace Laserfiche.Api.Client.Utils
{
    public static class DomainUtils
    {
        /// <summary>
        /// Returns the OAuth Api base uri using the given domain.
        /// </summary>
        /// <param name="domain">The Laserfiche domain.</param>
        /// <returns>The OAuth Api base uri.</returns>
        public static string GetOAuthApiBaseUri(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentNullException(nameof(domain));
            return $"https://signin.{domain}/oauth/";
        }

        /// <summary>
        /// Returns the Repository Api base uri using the given domain.
        /// </summary>
        /// <param name="domain">The Laserfiche domain.</param>
        /// <returns>The Repository Api base uri.</returns>
        public static string GetRepositoryApiBaseUri(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentNullException(nameof(domain));
            return $"https://api.{domain}/repository/";
        }
    }
}
