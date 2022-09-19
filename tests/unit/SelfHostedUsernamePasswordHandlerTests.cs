using Laserfiche.Api.Client.HttpHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Laserfiche.Api.Client.SelfHosted;
using System.Net;

namespace Laserfiche.Api.Client.UnitTest
{
    [TestClass]
    public class SelfHostedUsernamePasswordHandlerTests
    {
        private SelfHostedUsernamePasswordHandler _handler;
        private readonly string _username = "username";
        private readonly string _password = "password";
        private readonly string _baseUri = "http://localhost:11211";
        private readonly string _repoId = "repoId";
        private readonly string _grantType = "grantType";
        private readonly HttpRequestMessage _request = new();

        [TestMethod]
        public async Task BeforeSendAsync_NewToken_Success()
        {
            // Arrange
            string accessToken = "access_token";
            Mock<ITokenClient> tokenClientMock = new();
            tokenClientMock.Setup(mock => mock.TokenAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new SessionKeyInfo
            {
                Access_token = accessToken
            }));
            _handler = new SelfHostedUsernamePasswordHandler(_username, _password, _grantType, _repoId, _baseUri, tokenClientMock.Object);
            
            // Act
            var result = await _handler.BeforeSendAsync(_request, new CancellationToken());

            // Assert
            tokenClientMock.Verify(mock => mock.TokenAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<CancellationToken>()), Times.Once());
            Assert.AreEqual("Bearer", _request.Headers.Authorization.Scheme);
            Assert.IsFalse(string.IsNullOrEmpty(_request.Headers.Authorization.Parameter));
            Assert.IsNotNull(result);
            Assert.IsNull(result.RegionalDomain);
        }

        [TestMethod]
        public async Task BeforeSendAsync_ExistingToken_Success()
        {
            //Arrange
            string accessToken = "access_token";
            Mock<ITokenClient> tokenClientMock = new();
            tokenClientMock.Setup(mock => mock.TokenAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new SessionKeyInfo
            {
                Access_token = accessToken
            }));
            _handler = new SelfHostedUsernamePasswordHandler(_username, _password, _grantType, _repoId, _baseUri, tokenClientMock.Object);

            // Act
            var result = await _handler.BeforeSendAsync(_request, new CancellationToken());
            result = await _handler.BeforeSendAsync(_request, new CancellationToken());

            // Assert
            tokenClientMock.Verify(mock => mock.TokenAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<CancellationToken>()), Times.Once());
            Assert.AreEqual("Bearer", _request.Headers.Authorization.Scheme);
            Assert.IsFalse(string.IsNullOrEmpty(_request.Headers.Authorization.Parameter));
            Assert.IsNotNull(result);
            Assert.IsNull(result.RegionalDomain);
        }

        [TestMethod]
        public async Task BeforeSendAsync_FailedAuthentication_ThrowsException()
        {
            //Arrange
            string type = "accessDenied";
            string title = "Access token is invalid or expired.";
            int status = 401;

            Mock<ITokenClient> tokenClientMock = new();
            tokenClientMock.Setup(mock => mock.TokenAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<CancellationToken>())).Throws(new ApiException<ProblemDetails>(title, status, null, null, new ProblemDetails
            {
                Type = type,
                Title = title,
                Status = status
            }, null));
            _handler = new SelfHostedUsernamePasswordHandler(_username, _password, _grantType, _repoId, _baseUri, tokenClientMock.Object);

            // Assert
            var ex = await Assert.ThrowsExceptionAsync<ApiException<ProblemDetails>>(()=>_handler.BeforeSendAsync(_request, new CancellationToken()));
            Assert.AreEqual(type, ex.Result.Type);
            Assert.AreEqual(title, ex.Result.Title);
            Assert.AreEqual(status, ex.Result.Status);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.OK)]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.Forbidden)]
        [DataRow(HttpStatusCode.InternalServerError)]
        public async Task AfterSendAsync_ResponseOtherThanUnauthorized_ReturnsFalse(HttpStatusCode statusCode)
        {
            // Arrange
            HttpResponseMessage response = new()
            {
                StatusCode = statusCode,
            };
            _handler = new SelfHostedUsernamePasswordHandler(_username, _password, _grantType, _repoId, _baseUri);

            // Act
            var result = await _handler.AfterSendAsync(response, new CancellationToken());

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AfterSendAsync_ResponseUnauthorized_ReturnsTrue()
        {
            // Arrange
            HttpResponseMessage response = new()
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized,
            };
            _handler = new SelfHostedUsernamePasswordHandler(_username, _password, _grantType, _repoId, _baseUri);

            // Act
            var result = await _handler.AfterSendAsync(response, new CancellationToken());

            // Assert
            Assert.IsTrue(result);
        }
    }
}
