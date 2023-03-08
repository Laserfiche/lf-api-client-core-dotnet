using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Laserfiche.Api.Client
{
    /// <summary>
    /// A machine-readable format for specifying errors in HTTP API responses based on https://tools.ietf.org/html/rfc7807.
    /// </summary>
    public partial class ProblemDetails
    {
        internal const string OPERATION_ID_HEADER = "X-RequestId";
        internal const string API_SERVER_ERROR_HEADER = "X-APIServer-Error";

        /// <summary>
        /// The problem type.
        /// </summary>
        [JsonProperty("type", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        /// <summary>
        /// A short, human-readable summary of the problem type.
        /// </summary>
        [JsonProperty("title", Required = Required.Always)]
        public string Title { get; set; }

        /// <summary>
        /// The HTTP status code.
        /// </summary>
        [JsonProperty("status", Required = Required.Always)]
        public int Status { get; set; }

        /// <summary>
        /// A human-readable explanation specific to this occurrence of the problem.
        /// </summary>
        [JsonProperty("detail", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Detail { get; set; }

        /// <summary>
        /// A URI reference that identifies the specific occurrence of the problem.
        /// </summary>
        [JsonProperty("instance", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Instance { get; set; }

        /// <summary>
        /// The operation id.
        /// </summary>
        [JsonProperty("operationId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string OperationId { get; set; }

        /// <summary>
        /// The error source.
        /// </summary>
        [JsonProperty("errorSource", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorSource { get; set; }

        /// <summary>
        /// The error code.
        /// </summary>
        [JsonProperty("errorCode", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public int ErrorCode { get; set; }

        /// <summary>
        /// The trace id.
        /// </summary>
        [JsonProperty("traceId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string TraceId { get; set; }

        /// <summary>
        /// The extension members.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, object> Extensions { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Create a minimal <see cref="ProblemDetails"/> using HTTP response information.
        /// </summary>
        /// <param name="statusCode">The response status code.</param>
        /// <param name="headers">The response headers.</param>
        /// <returns>ProblemDetails</returns>
        public static ProblemDetails Create(int statusCode, IReadOnlyDictionary<string, IEnumerable<string>> headers)
        {
            return new ProblemDetails
            {
                Status = statusCode,
                Title = headers?.TryGetValue(API_SERVER_ERROR_HEADER, out IEnumerable<string> apiServerErrorHeader) == true ? WebUtility.UrlDecode(apiServerErrorHeader?.FirstOrDefault()) : $"HTTP status code {statusCode}.",
                OperationId = headers?.TryGetValue(OPERATION_ID_HEADER, out IEnumerable<string> operationIdHeader) == true ? operationIdHeader.FirstOrDefault() : null
            };
        }
    }
}
