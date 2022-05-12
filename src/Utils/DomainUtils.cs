using System;

namespace Laserfiche.Api.Client.Utils
{
    public static class DomainUtils
    {
        /// <summary>
        /// Returns the Laserfiche domain using the Laserfiche account id.
        /// </summary>
        /// <param name="accountId">The Laserfiche account id.</param>
        /// <returns>The Laserfiche domain.</returns>
        public static string GetDomainFromAccountId(string accountId)
        {
            if (accountId?.Length == 10)
            {
                if (accountId.StartsWith("1"))
                {
                    return "laserfiche.ca";
                }
                else if (accountId.StartsWith("2"))
                {
                    return "eu.laserfiche.com";
                }
            }
            else if (accountId?.Length == 9)
            {
                return "laserfiche.com";
            }
            throw new ArgumentOutOfRangeException(nameof(accountId));
        }

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
