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
            if (string.IsNullOrEmpty(configuration.CustomerId))
            {
                throw new ArgumentException(Resources.Strings.INVALID_ACCOUNT_ID, nameof(configuration.CustomerId));
            }

            if (string.IsNullOrEmpty(configuration.Domain))
            {
                throw new ArgumentException(Resources.Strings.INVALID_DOMAIN, nameof(configuration.Domain));
            }

            if (string.IsNullOrEmpty(configuration.ClientId))
            {
                throw new ArgumentException(Resources.Strings.INVALID_CLIENT_ID, nameof(configuration.ClientId));
            }

            if (string.IsNullOrEmpty(configuration.ServicePrincipalKey))
            {
                throw new ArgumentException(Resources.Strings.INVALID_SERVICE_PRINCIPAL_KEY, nameof(configuration.ServicePrincipalKey));
            }

            bool isValidSigningKey = configuration.Jwk != null && configuration.Jwk.KeySize != 0;
            if (!isValidSigningKey)
            {
                throw new ArgumentException(Resources.Strings.INVALID_ACCESS_KEY, nameof(configuration.Jwk));
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
