// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Api.Client.HttpHandlers
{
    /// <summary>
    /// Provides a way to modify an HTTP request and to handle the response.
    /// </summary>
    public interface IHttpRequestHandler
    {
        /// <summary>
        /// Invoked before an HTTP request with the request message and cancellation token.
        /// </summary>
        /// <param name="httpRequestMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>BeforeSendResult</returns>
        Task<BeforeSendResult> BeforeSendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Invoked after an HTTP request with the response message and cancellation token.
        /// </summary>
        /// <param name="httpResponseMessage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if the request should be retried.</returns>
        Task<bool> AfterSendAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken);
    }

    public class BeforeSendResult
    {
        /// <summary>
        /// Laserfiche Cloud regional domain.
        /// </summary>
        public string RegionalDomain { get; set; }
    }
}
