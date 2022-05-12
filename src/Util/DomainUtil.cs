namespace Laserfiche.Api.Client.Util
{
    public static class DomainUtil
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
            return "laserfiche.com";
        }

        /// <summary>
        /// Returns the OAuth base uri using the given domain.
        /// </summary>
        /// <param name="domain">The Laserfiche domain.</param>
        /// <returns>The OAuth base uri.</returns>
        public static string GetOAuthBaseUri(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                domain = "laserfiche.com";
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
                domain = "laserfiche.com";
            return $"https://api.{domain}/repository/";
        }
    }
}
