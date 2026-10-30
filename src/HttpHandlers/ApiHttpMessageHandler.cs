// Copyright Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client.Utils;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Api.Client.HttpHandlers
{
    /// <summary>
    /// HttpClient Message Handler to interact with Laserfiche APIs. It provides the following features:
    /// - Uses the IHttpRequestHandler to obtain the value of the Authorization header and refresh it as needed.
    /// - Sets the Api Base Address based on the Laserfiche cloud region Access Token.
    /// - Built-in retries on 401 and transient errors. A transient error is retried once, after the delay a 429 or 503
    ///   asks for in its Retry-After header (up to <see cref="MaxRetryAfter"/>), or after a short random delay.
    /// </summary>
    public class ApiHttpMessageHandler : DelegatingHandler
    {
        /// <summary>The longest Retry-After the handler waits for. A response asking for longer is returned to the caller.</summary>
        public static readonly TimeSpan MaxRetryAfter = TimeSpan.FromSeconds(30);

        // The random delay before retrying a transient error that carries no Retry-After, so that many clients failing
        // at the same moment do not all retry at the same moment.
        internal static readonly TimeSpan MinTransientRetryDelay = TimeSpan.FromMilliseconds(200);
        internal static readonly TimeSpan MaxTransientRetryDelay = TimeSpan.FromMilliseconds(1000);

        private static readonly Random Jitter = new Random();

        public delegate string GetApiBaseUri(string laserficheCloudRegionalDomain);
        private readonly IHttpRequestHandler _httpRequestHandler;
        private readonly GetApiBaseUri _getApiBaseUri;
        private readonly Func<TimeSpan, CancellationToken, Task> _delay;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="httpRequestHandler"></param>
        /// <param name="getApiBaseUri">Typical argument values: <see cref="DomainUtils.GetRepositoryApiBaseUri"/>, <see cref="DomainUtils.GetODataApiBaseUri"/>, or a function returning a custom API base address.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiHttpMessageHandler(IHttpRequestHandler httpRequestHandler, GetApiBaseUri getApiBaseUri) :
          this(httpRequestHandler, getApiBaseUri, new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip }, Task.Delay)
        {
        }

        // For tests: the inner handler answers instead of the network, and the delay is recorded instead of waited.
        internal ApiHttpMessageHandler(IHttpRequestHandler httpRequestHandler, GetApiBaseUri getApiBaseUri, HttpMessageHandler innerHandler,
            Func<TimeSpan, CancellationToken, Task> delay) :
          base(innerHandler)
        {
            _httpRequestHandler = httpRequestHandler ?? throw new ArgumentNullException(nameof(httpRequestHandler));
            _getApiBaseUri = getApiBaseUri ?? throw new ArgumentNullException(nameof(getApiBaseUri));
            _delay = delay ?? throw new ArgumentNullException(nameof(delay));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var first = await SendExAsync(request, cancellationToken, true).ConfigureAwait(false);
            if (first.Response != null)
            {
                return first.Response;
            }
            if (first.RetryDelay > TimeSpan.Zero)
            {
                await _delay(first.RetryDelay, cancellationToken).ConfigureAwait(false);
            }
            return (await SendExAsync(request, cancellationToken, false).ConfigureAwait(false)).Response;
        }

        /// <summary>
        /// Sends the request. When <paramref name="returnNullIfRetriable"/> is true and the request should be retried, the
        /// response is null and RetryDelay says how long to wait first.
        /// </summary>
        private async Task<(HttpResponseMessage Response, TimeSpan RetryDelay)> SendExAsync(HttpRequestMessage request, CancellationToken cancellationToken, bool returnNullIfRetriable)
        {
            HttpResponseMessage response;

            // Sets the authorization header
            var beforeSendResult = await _httpRequestHandler.BeforeSendAsync(request, cancellationToken).ConfigureAwait(false);
            string apiBaseUri = _getApiBaseUri(beforeSendResult.RegionalDomain);
            if (!request.RequestUri.AbsoluteUri.StartsWith(apiBaseUri, StringComparison.OrdinalIgnoreCase))
            {
                request.RequestUri = new Uri(string.Concat(apiBaseUri, request.RequestUri.PathAndQuery.Substring(1)));
            }

            try
            {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch when (returnNullIfRetriable && !cancellationToken.IsCancellationRequested)
            {
                // A transport failure: retried once, after a short random delay. A cancelled call is not retried.
                return (null, TransientRetryDelay());
            }

            if (returnNullIfRetriable && IsIdempotentHttpMethod(request.Method))
            {
                TimeSpan? retryDelay = GetRetryDelay(response);
                if (retryDelay.HasValue)
                {
                    response.Dispose();
                    return (null, retryDelay.Value);
                }
            }

            bool shouldRetry = await _httpRequestHandler.AfterSendAsync(response, cancellationToken).ConfigureAwait(false);
            if (returnNullIfRetriable && shouldRetry)
            {
                response.Dispose();
                return (null, TimeSpan.Zero);
            }
            return (response, TimeSpan.Zero);
        }

        /// <summary>
        /// How long to wait before retrying <paramref name="response"/>, or null if it should not be retried. A 429 or 503
        /// with Retry-After is retried after that delay, unless the delay is longer than <see cref="MaxRetryAfter"/>; then
        /// the caller gets the response and decides. Any other 5xx, or 408, is retried after a short random delay. A 429
        /// without Retry-After is not retried.
        /// </summary>
        internal static TimeSpan? GetRetryDelay(HttpResponseMessage response)
        {
            HttpStatusCode statusCode = response.StatusCode;
            bool asksToWait = statusCode == (HttpStatusCode)429 || statusCode == HttpStatusCode.ServiceUnavailable;
            TimeSpan? retryAfter = asksToWait ? GetRetryAfter(response) : null;
            if (retryAfter.HasValue)
            {
                return retryAfter.Value <= MaxRetryAfter ? retryAfter : null;
            }
            if (IsTransientHttpStatusCode(statusCode))
            {
                return TransientRetryDelay();
            }
            return null;
        }

        private static TimeSpan? GetRetryAfter(HttpResponseMessage response)
        {
            var retryAfter = response.Headers.RetryAfter;
            if (retryAfter?.Delta != null)
            {
                return retryAfter.Delta.Value < TimeSpan.Zero ? TimeSpan.Zero : retryAfter.Delta.Value;
            }
            if (retryAfter?.Date != null)
            {
                TimeSpan untilDate = retryAfter.Date.Value - DateTimeOffset.UtcNow;
                return untilDate < TimeSpan.Zero ? TimeSpan.Zero : untilDate;
            }
            return null;
        }

        private static TimeSpan TransientRetryDelay()
        {
            double fraction;
            lock (Jitter)
            {
                fraction = Jitter.NextDouble();
            }
            return MinTransientRetryDelay + TimeSpan.FromTicks((long)((MaxTransientRetryDelay - MinTransientRetryDelay).Ticks * fraction));
        }

        private static bool IsTransientHttpStatusCode(HttpStatusCode statusCode)
        {
            return (int)statusCode >= 500 || statusCode == HttpStatusCode.RequestTimeout;
        }

        private static bool IsIdempotentHttpMethod(HttpMethod method)
        {
            return method == HttpMethod.Get || method == HttpMethod.Put || method == HttpMethod.Options;
        }
    }
}
