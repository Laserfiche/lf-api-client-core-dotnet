using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Laserfiche.Api.Client
{
    public partial class ApiException : Exception
    {
        public const string OPERATION_ID_HEADER = "X-RequestId";

        /// <summary>
        /// The API status code.
        /// </summary>
        public int StatusCode { get; private set; }

        /// <summary>
        /// The API error details.
        /// </summary>
        public ProblemDetails ProblemDetails { get; private set; }

        /// <summary>
        /// The API response headers.
        /// </summary>
        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }

        public ApiException(string message, int statusCode, IReadOnlyDictionary<string, IEnumerable<string>> headers, ProblemDetails problemDetails, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            Headers = headers;
            ProblemDetails = problemDetails;
        }

        /// <summary>
        /// Create an <see cref="ApiException"/>. Will try to deserialize the <paramref name="response"/> to a <see cref="Client.ProblemDetails"/>.
        /// </summary>
        /// <param name="statusCode">The response status code.</param>
        /// <param name="headers">The response headers.</param>
        /// <param name="response">The response body.</param>
        /// <param name="jsonSerializerSettings">The json serializer settings used to deserialize the response.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <returns></returns>
        public static ApiException Create(int statusCode, IReadOnlyDictionary<string, IEnumerable<string>> headers, string response, JsonSerializerSettings jsonSerializerSettings, Exception innerException)
        {
            if (!string.IsNullOrWhiteSpace(response))
            {
                try
                {
                    ProblemDetails problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(response, jsonSerializerSettings);
                    return Create(statusCode, headers, problemDetails, innerException);
                }
                catch
                {
                    // fail to deserialize to ProblemDetails silently
                }
            }

            return Create(statusCode, headers, innerException);
        }

        /// <summary>
        /// Create an <see cref="ApiException"/> with a <see cref="Client.ProblemDetails"/>.
        /// </summary>
        /// <param name="statusCode">The response status code.</param>
        /// <param name="headers">The response headers.</param>
        /// <param name="problemDetails">The <see cref="Client.ProblemDetails"/> response.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <returns></returns>
        public static ApiException Create(int statusCode, IReadOnlyDictionary<string, IEnumerable<string>> headers, ProblemDetails problemDetails, Exception innerException)
        {
            if (problemDetails == null)
            {
                return Create(statusCode, headers, innerException);
            }

            return new ApiException(problemDetails.Title, statusCode, headers, problemDetails, innerException);
        }

        /// <summary>
        /// Create an <see cref="ApiException"/>.
        /// </summary>
        /// <param name="statusCode">The response status code.</param>
        /// <param name="headers">The response headers.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <returns></returns>
        public static ApiException Create(int statusCode, IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception innerException)
        {
            ProblemDetails problemDetails = new ProblemDetails()
            {
                Title = $"HTTP status code {statusCode}.",
                Status = statusCode,
                OperationId = headers?.TryGetValue(OPERATION_ID_HEADER, out IEnumerable<string> operationIdHeader) == true ? operationIdHeader.FirstOrDefault() : null,
            };
            return Create(statusCode, headers, problemDetails, innerException);
        }
    }
}
