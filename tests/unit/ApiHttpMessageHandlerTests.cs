// Copyright Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client.HttpHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Laserfiche.Api.Client.UnitTest
{
    [TestClass]
    public class ApiHttpMessageHandlerTests
    {
        private const string BaseUri = "https://api.example.com/repository/";

        private sealed class ScriptedHandler : HttpMessageHandler
        {
            private readonly Queue<Func<HttpResponseMessage>> responses = new Queue<Func<HttpResponseMessage>>();
            public int Calls { get; private set; }

            public ScriptedHandler Then(Func<HttpResponseMessage> response)
            {
                responses.Enqueue(response);
                return this;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Calls++;
                return Task.FromResult(responses.Dequeue()());
            }
        }

        private static HttpResponseMessage Status(HttpStatusCode status, TimeSpan? retryAfter = null)
        {
            var response = new HttpResponseMessage(status);
            if (retryAfter.HasValue)
                response.Headers.RetryAfter = new RetryConditionHeaderValue(retryAfter.Value);
            return response;
        }

        private static (HttpMessageInvoker invoker, List<TimeSpan> delays) Create(ScriptedHandler inner)
        {
            var requestHandler = new Mock<IHttpRequestHandler>();
            requestHandler.Setup(h => h.BeforeSendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BeforeSendResult { RegionalDomain = "example.com" });
            requestHandler.Setup(h => h.AfterSendAsync(It.IsAny<HttpResponseMessage>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var delays = new List<TimeSpan>();
            var handler = new ApiHttpMessageHandler(requestHandler.Object, _ => BaseUri, inner, (delay, token) =>
            {
                delays.Add(delay);
                return Task.CompletedTask;
            });
            return (new HttpMessageInvoker(handler), delays);
        }

        private static HttpRequestMessage Get() => new HttpRequestMessage(HttpMethod.Get, BaseUri + "v2/Repositories");

        [TestMethod]
        public async Task ServiceUnavailableWithRetryAfter_RetriedOnceAfterThatDelay()
        {
            var inner = new ScriptedHandler().Then(() => Status(HttpStatusCode.ServiceUnavailable, TimeSpan.FromSeconds(10))).Then(() => Status(HttpStatusCode.OK));
            var (invoker, delays) = Create(inner);

            var response = await invoker.SendAsync(Get(), CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(2, inner.Calls);
            CollectionAssert.AreEqual(new[] { TimeSpan.FromSeconds(10) }, delays);
        }

        [TestMethod]
        public async Task TooManyRequestsWithRetryAfter_RetriedOnceAfterThatDelay()
        {
            var inner = new ScriptedHandler().Then(() => Status((HttpStatusCode)429, TimeSpan.FromSeconds(5))).Then(() => Status(HttpStatusCode.OK));
            var (invoker, delays) = Create(inner);

            var response = await invoker.SendAsync(Get(), CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            CollectionAssert.AreEqual(new[] { TimeSpan.FromSeconds(5) }, delays);
        }

        [TestMethod]
        public async Task ServiceUnavailableWithoutRetryAfter_RetriedAfterAShortRandomDelay()
        {
            var inner = new ScriptedHandler().Then(() => Status(HttpStatusCode.ServiceUnavailable)).Then(() => Status(HttpStatusCode.OK));
            var (invoker, delays) = Create(inner);

            await invoker.SendAsync(Get(), CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(1, delays.Count);
            Assert.IsTrue(delays[0] >= ApiHttpMessageHandler.MinTransientRetryDelay && delays[0] <= ApiHttpMessageHandler.MaxTransientRetryDelay, delays[0].ToString());
        }

        [TestMethod]
        public async Task RetryAfterAboveTheCap_NotRetried_ResponseReturned()
        {
            var inner = new ScriptedHandler().Then(() => Status(HttpStatusCode.ServiceUnavailable, ApiHttpMessageHandler.MaxRetryAfter + TimeSpan.FromSeconds(1)));
            var (invoker, delays) = Create(inner);

            var response = await invoker.SendAsync(Get(), CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            Assert.AreEqual(1, inner.Calls);
            Assert.AreEqual(0, delays.Count);
        }

        [TestMethod]
        public async Task TooManyRequestsWithoutRetryAfter_NotRetried()
        {
            var inner = new ScriptedHandler().Then(() => Status((HttpStatusCode)429));
            var (invoker, _) = Create(inner);

            var response = await invoker.SendAsync(Get(), CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual((HttpStatusCode)429, response.StatusCode);
            Assert.AreEqual(1, inner.Calls);
        }

        [TestMethod]
        public async Task NonIdempotentMethods_NotRetriedOnA5xx()
        {
            foreach (var method in new[] { HttpMethod.Post, new HttpMethod("PATCH"), HttpMethod.Delete })
            {
                var inner = new ScriptedHandler().Then(() => Status(HttpStatusCode.ServiceUnavailable, TimeSpan.FromSeconds(1)));
                var (invoker, _) = Create(inner);

                var response = await invoker.SendAsync(new HttpRequestMessage(method, BaseUri + "v2/x"), CancellationToken.None).ConfigureAwait(false);

                Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode, method.Method);
                Assert.AreEqual(1, inner.Calls, method.Method);
            }
        }

        [TestMethod]
        public async Task SecondAttemptFails_ItsResponseIsReturned()
        {
            var inner = new ScriptedHandler().Then(() => Status(HttpStatusCode.InternalServerError)).Then(() => Status(HttpStatusCode.InternalServerError));
            var (invoker, _) = Create(inner);

            var response = await invoker.SendAsync(Get(), CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual(2, inner.Calls, "one retry only");
        }

        [TestMethod]
        public async Task CancelledCall_NotRetried()
        {
            var cancelled = new CancellationTokenSource();
            cancelled.Cancel();
            var inner = new ScriptedHandler().Then(() => throw new TaskCanceledException());
            var (invoker, delays) = Create(inner);

            await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => invoker.SendAsync(Get(), cancelled.Token)).ConfigureAwait(false);
            Assert.AreEqual(1, inner.Calls);
            Assert.AreEqual(0, delays.Count);
        }

        [TestMethod]
        public async Task TransportFailure_Get_RetriedOnce()
        {
            var inner = new ScriptedHandler().Then(() => throw new HttpRequestException("reset")).Then(() => Status(HttpStatusCode.OK));
            var (invoker, delays) = Create(inner);

            var response = await invoker.SendAsync(Get(), CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(1, delays.Count);
        }

        [TestMethod]
        public async Task TransportFailure_Post_NotRetried()
        {
            // The server may have created the entry before the connection failed.
            var inner = new ScriptedHandler().Then(() => throw new HttpRequestException("reset"));
            var (invoker, _) = Create(inner);

            await Assert.ThrowsExceptionAsync<HttpRequestException>(() =>
                invoker.SendAsync(new HttpRequestMessage(HttpMethod.Post, BaseUri + "v2/x") { Content = new StringContent("{}") }, CancellationToken.None)).ConfigureAwait(false);
            Assert.AreEqual(1, inner.Calls);
        }

        [TestMethod]
        public async Task StreamBody_NotRetried_ResponseKept()
        {
            // A stream body may already have been read; the caller keeps the 503 and its problem details.
            var inner = new ScriptedHandler().Then(() => Status(HttpStatusCode.ServiceUnavailable, TimeSpan.FromSeconds(1)));
            var (invoker, delays) = Create(inner);
            var put = new HttpRequestMessage(HttpMethod.Put, BaseUri + "v2/x") { Content = new StreamContent(new System.IO.MemoryStream(new byte[] { 1, 2, 3 })) };

            var response = await invoker.SendAsync(put, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            Assert.AreEqual(1, inner.Calls);
            Assert.AreEqual(0, delays.Count);
        }

        [TestMethod]
        public async Task BufferedBody_Put_Retried()
        {
            var inner = new ScriptedHandler().Then(() => Status(HttpStatusCode.ServiceUnavailable, TimeSpan.FromSeconds(1))).Then(() => Status(HttpStatusCode.OK));
            var (invoker, _) = Create(inner);
            var put = new HttpRequestMessage(HttpMethod.Put, BaseUri + "v2/x") { Content = new StringContent("{}") };

            var response = await invoker.SendAsync(put, CancellationToken.None).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void GetRetryDelay_RetryAfterAsADate()
        {
            var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddSeconds(20));

            TimeSpan? delay = ApiHttpMessageHandler.GetRetryDelay(response);

            Assert.IsTrue(delay > TimeSpan.FromSeconds(15) && delay <= TimeSpan.FromSeconds(20), delay.ToString());
        }
    }
}
