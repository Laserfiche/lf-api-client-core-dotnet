using Laserfiche.Api.Client.HttpHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Laserfiche.Api.Client.APIServer;
using System.Net;

namespace Laserfiche.Api.Client.UnitTest
{
    [TestClass]
    public class UsernamePasswordHandlerTests
    {
        private UsernamePasswordHandler _handler;
        private readonly string _repoId = "repoId";
        private readonly string _username = "username";
        private readonly string _password = "password";
        private readonly string _baseUrl = "http://localhost:11211";
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
            _handler = new UsernamePasswordHandler(_repoId, _username, _password, _baseUrl, tokenClientMock.Object);
            
            // Act
            var result = await _handler.BeforeSendAsync(_request, new CancellationToken()).ConfigureAwait(false);

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
            _handler = new UsernamePasswordHandler(_repoId, _username, _password, _baseUrl, tokenClientMock.Object);

            // Act
            var result = await _handler.BeforeSendAsync(_request, new CancellationToken()).ConfigureAwait(false);
            result = await _handler.BeforeSendAsync(_request, new CancellationToken()).ConfigureAwait(false);

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
            var problemDetails = new ProblemDetails
            {
                Type = type,
                Title = title,
                Status = status
            };
            tokenClientMock.Setup(mock => mock.TokenAsync(It.IsAny<string>(), It.IsAny<CreateConnectionRequest>(), It.IsAny<CancellationToken>())).Throws(ApiException.Create(status, null, problemDetails, null));
            _handler = new UsernamePasswordHandler(_repoId, _username, _password, _baseUrl, tokenClientMock.Object);

            // Assert
            var ex = await Assert.ThrowsExceptionAsync<ApiException>(() => _handler.BeforeSendAsync(_request, new CancellationToken())).ConfigureAwait(false);
            Assert.AreEqual(type, ex.ProblemDetails.Type);
            Assert.AreEqual(title, ex.ProblemDetails.Title);
            Assert.AreEqual(status, ex.ProblemDetails.Status);
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
            _handler = new UsernamePasswordHandler(_repoId, _username, _password, _baseUrl);

            // Act
            var result = await _handler.AfterSendAsync(response, new CancellationToken()).ConfigureAwait(false);

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
            _handler = new UsernamePasswordHandler(_repoId, _username, _password, _baseUrl);

            // Act
            var result = await _handler.AfterSendAsync(response, new CancellationToken()).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
