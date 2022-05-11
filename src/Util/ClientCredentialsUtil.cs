using Laserfiche.Oauth.Token.Client;
using System;

namespace Laserfiche.Oauth.Api.Client.Util
{
    internal static class ClientCredentialsUtil
    {
        internal static void IsValid(this AccessKey accessKey)
        {
            if (string.IsNullOrEmpty(accessKey.CustomerId))
            {
                throw new ArgumentException(Resources.Strings.INVALID_CUSTOMER_ID, nameof(accessKey.CustomerId));
            }

            if (string.IsNullOrEmpty(accessKey.Domain))
            {
                throw new ArgumentException(Resources.Strings.INVALID_DOMAIN, nameof(accessKey.Domain));
            }

            if (string.IsNullOrEmpty(accessKey.ClientId))
            {
                throw new ArgumentException(Resources.Strings.INVALID_CLIENT_ID, nameof(accessKey.ClientId));
            }

            bool isValidSigningKey = accessKey.Jwk != null && accessKey.Jwk.KeySize != 0;
            if (!isValidSigningKey)
            {
                throw new ArgumentException(Resources.Strings.INVALID_ACCESS_KEY, nameof(accessKey.Jwk));
            }
        }
    }
}
