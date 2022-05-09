using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Oauth.Api.Client
{
    // Can be put any where as long as we want to modify http request/response. Inject behavior into req/res pipeline.
    public interface IHttpRequestHandler
    {
        /// <summary>
        /// Invoked before an HTTP request with the request message and cancellation token.
        /// Returns BeforeSendResult.
        /// </summary>
        Task<BeforeSendResult> BeforeSendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken);


        /// <summary>
        /// Invoked after a request with the response message, the repository client and cancellation token.
        /// Returns true if the request should be retried.
        /// </summary>
        Task<bool> AfterSendAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken);
    }

    public class BeforeSendResult
    {
        public string RegionalDomain { get; set; }
    }
}
