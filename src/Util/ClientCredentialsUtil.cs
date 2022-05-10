using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Laserfiche.Oauth.Api.Client.Util
{
    internal static class ClientCredentialsUtil
    {
        internal static FormUrlEncodedContent RequestToFormUrlEncodedContent(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var kvp = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return new FormUrlEncodedContent(kvp);
        }

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

        internal static Exception GetStandardErrorException(string responseContent, HttpStatusCode responseStatusCode)
        {
            var oauthProblemDetails = JsonConvert.DeserializeObject<OAuthProblemDetails>(responseContent);

            if (oauthProblemDetails is null)
            {
                var exception = new Exception(Resources.Strings.UNABLE_TO_READ_RESPONSE);
                exception.Data[nameof(oauthProblemDetails.Status)] = responseStatusCode.ToString();
                return exception;
            }
            else
            {
                var exception = new Exception(oauthProblemDetails.ToString());
                exception.Data[nameof(oauthProblemDetails.Type)] = oauthProblemDetails.Type.ToString();
                exception.Data[nameof(oauthProblemDetails.Title)] = oauthProblemDetails.Title.ToString();
                exception.Data[nameof(oauthProblemDetails.Status)] = oauthProblemDetails.Status.ToString();
                exception.Data[nameof(oauthProblemDetails.Instance)] = oauthProblemDetails.Instance.ToString();
                exception.Data[nameof(oauthProblemDetails.OperationId)] = oauthProblemDetails.OperationId.ToString();
                return exception;
            }
        }
    }
}
