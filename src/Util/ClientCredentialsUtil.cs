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

        internal static void IsValid(this ClientCredentialsOptions configuration)
        {
            if (string.IsNullOrEmpty(configuration.AccessKey.CustomerId))
            {
                throw new ArgumentException(Resources.Strings.INVALID_CUSTOMER_ID, nameof(configuration.AccessKey.CustomerId));
            }

            if (string.IsNullOrEmpty(configuration.AccessKey.Domain))
            {
                throw new ArgumentException(Resources.Strings.INVALID_DOMAIN, nameof(configuration.AccessKey.Domain));
            }

            if (string.IsNullOrEmpty(configuration.AccessKey.ClientId))
            {
                throw new ArgumentException(Resources.Strings.INVALID_CLIENT_ID, nameof(configuration.AccessKey.ClientId));
            }

            if (string.IsNullOrEmpty(configuration.ServicePrincipalKey))
            {
                throw new ArgumentException(Resources.Strings.INVALID_SERVICE_PRINCIPAL_KEY, nameof(configuration.ServicePrincipalKey));
            }

            bool isValidSigningKey = configuration.AccessKey.Jwk != null && configuration.AccessKey.Jwk.KeySize != 0;
            if (!isValidSigningKey)
            {
                throw new ArgumentException(Resources.Strings.INVALID_ACCESS_KEY, nameof(configuration.AccessKey.Jwk));
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
