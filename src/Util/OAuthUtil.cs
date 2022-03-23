namespace Laserfiche.Oauth.Api.Client.Util
{
    internal static class OAuthUtil
    {
        internal static string GetOauthUri(string address, IClientCredentialsOptions config, bool addBase = true)
        {
            string newAddress = address;
            string baseAddress = config.BaseAddress.TrimEnd('/') + "/";
            if (addBase)
            {
                newAddress = $"{baseAddress}{newAddress}";
            }
            return newAddress;
        }
    }
}
