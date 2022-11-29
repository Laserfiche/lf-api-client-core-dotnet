using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Laserfiche.Api.Client.UnitTest
{
    [TestClass]
    public class ApiExceptionTests
    {
        private void AssertNullProblemDetailsOptionalProperties(ProblemDetails problemDetails)
        {
            Assert.IsNull(problemDetails.Type);
            Assert.IsNull(problemDetails.Detail);
            Assert.IsNull(problemDetails.Instance);
            Assert.AreEqual(default, problemDetails.ErrorCode);
            Assert.IsNull(problemDetails.ErrorSource);
            Assert.IsNull(problemDetails.TraceId);
        }

        [TestMethod]
        public void Create_WithNullHeaders_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;

            ApiException exception = ApiException.Create(statusCode, null, null);

            Assert.AreEqual($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.AreEqual(statusCode, exception.ProblemDetails.Status);
            Assert.IsNull(exception.ProblemDetails.OperationId);
            Assert.AreEqual(0, exception.ProblemDetails.Extensions.Count);
            Assert.AreEqual(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.AreEqual(exception.ProblemDetails.Title, exception.Message);
            Assert.IsNull(exception.Headers);
            Assert.IsNull(exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
        }

        [TestMethod]
        public void Create_WithoutOperationIdHeader_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                ["Content-Type"] = new List<string>() { "application/json" }
            };

            ApiException exception = ApiException.Create(statusCode, headers, null);

            Assert.AreEqual($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.AreEqual(statusCode, exception.ProblemDetails.Status);
            Assert.IsNull(exception.ProblemDetails.OperationId);
            Assert.AreEqual(0, exception.ProblemDetails.Extensions.Count);
            Assert.AreEqual(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.AreEqual(exception.ProblemDetails.Title, exception.Message);
            Assert.AreEqual(headers, exception.Headers);
            Assert.IsNull(exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
        }

        [TestMethod]
        public void Create_WithOperationIdHeader_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ProblemDetails.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };

            ApiException exception = ApiException.Create(statusCode, headers, null);

            Assert.AreEqual($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.AreEqual(statusCode, exception.ProblemDetails.Status);
            Assert.AreEqual(operationId, exception.ProblemDetails.OperationId);
            Assert.AreEqual(0, exception.ProblemDetails.Extensions.Count);
            Assert.AreEqual(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.AreEqual(exception.ProblemDetails.Title, exception.Message);
            Assert.AreEqual(headers, exception.Headers);
            Assert.IsNull(exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
        }

        [TestMethod]
        public void Create_WithInnerException_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ProblemDetails.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiException.Create(statusCode, headers, innerException);

            Assert.AreEqual($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.AreEqual(statusCode, exception.ProblemDetails.Status);
            Assert.AreEqual(operationId, exception.ProblemDetails.OperationId);
            Assert.AreEqual(0, exception.ProblemDetails.Extensions.Count);
            Assert.AreEqual(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.AreEqual(exception.ProblemDetails.Title, exception.Message);
            Assert.AreEqual(headers, exception.Headers);
            Assert.AreEqual(innerException, exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
        }

        [TestMethod]
        public void Create_WithProblemDetails_ExceptionHasFullProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ProblemDetails.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            ProblemDetails problemDetails = new ProblemDetails()
            {
                Title = "An error occurred.",
                Type = "ErrorType",
                Detail = "Detail",
                Status = statusCode,
                Instance = "Instance",
                OperationId = operationId,
                ErrorCode = 123,
                ErrorSource = "ErrorSource",
                TraceId = "TraceId",
                Extensions = new Dictionary<string, object>() { ["key"] = "value" }
            };
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiException.Create(statusCode, headers, problemDetails, innerException);

            Assert.AreEqual(problemDetails.Title, exception.ProblemDetails.Title);
            Assert.AreEqual(problemDetails.Type, exception.ProblemDetails.Type);
            Assert.AreEqual(problemDetails.Detail, exception.ProblemDetails.Detail);
            Assert.AreEqual(problemDetails.Status, exception.ProblemDetails.Status);
            Assert.AreEqual(problemDetails.Instance, exception.ProblemDetails.Instance);
            Assert.AreEqual(problemDetails.OperationId, exception.ProblemDetails.OperationId);
            Assert.AreEqual(problemDetails.ErrorCode, exception.ProblemDetails.ErrorCode);
            Assert.AreEqual(problemDetails.ErrorSource, exception.ProblemDetails.ErrorSource);
            Assert.AreEqual(problemDetails.TraceId, exception.ProblemDetails.TraceId);
            Assert.AreEqual(problemDetails.Extensions, exception.ProblemDetails.Extensions);
            Assert.AreEqual(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.AreEqual(exception.ProblemDetails.Title, exception.Message);
            Assert.AreEqual(headers, exception.Headers);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [TestMethod]
        public void Create_WithNullProblemDetails_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ProblemDetails.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            ProblemDetails problemDetails = null;
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiException.Create(statusCode, headers, problemDetails, innerException);

            Assert.AreEqual($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.AreEqual(statusCode, exception.ProblemDetails.Status);
            Assert.AreEqual(operationId, exception.ProblemDetails.OperationId);
            Assert.AreEqual(0, exception.ProblemDetails.Extensions.Count);
            Assert.AreEqual(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.AreEqual(exception.ProblemDetails.Title, exception.Message);
            Assert.AreEqual(headers, exception.Headers);
            Assert.AreEqual(innerException, exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
        }

        [TestMethod]
        public void Create_WithResponseString_ResponseStringIsProblemDetails_ExceptionHasFullProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            string extensionsKey = "key";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ProblemDetails.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            ProblemDetails problemDetails = new ProblemDetails()
            {
                Title = "An error occurred.",
                Type = "ErrorType",
                Detail = "Detail",
                Status = statusCode,
                Instance = "Instance",
                OperationId = operationId,
                ErrorCode = 123,
                ErrorSource = "ErrorSource",
                TraceId = "TraceId",
                Extensions = new Dictionary<string, object>() { [extensionsKey] = "value" }
            };
            string problemDetailsResponseString = JsonConvert.SerializeObject(problemDetails);
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiException.Create(statusCode, headers, problemDetailsResponseString, null, innerException);

            Assert.AreEqual(problemDetails.Title, exception.ProblemDetails.Title);
            Assert.AreEqual(problemDetails.Type, exception.ProblemDetails.Type);
            Assert.AreEqual(problemDetails.Detail, exception.ProblemDetails.Detail);
            Assert.AreEqual(problemDetails.Status, exception.ProblemDetails.Status);
            Assert.AreEqual(problemDetails.Instance, exception.ProblemDetails.Instance);
            Assert.AreEqual(problemDetails.OperationId, exception.ProblemDetails.OperationId);
            Assert.AreEqual(problemDetails.ErrorCode, exception.ProblemDetails.ErrorCode);
            Assert.AreEqual(problemDetails.ErrorSource, exception.ProblemDetails.ErrorSource);
            Assert.AreEqual(problemDetails.TraceId, exception.ProblemDetails.TraceId);
            Assert.AreEqual(problemDetails.Extensions.Count, exception.ProblemDetails.Extensions.Count);
            Assert.AreEqual(problemDetails.Extensions[extensionsKey], exception.ProblemDetails.Extensions[extensionsKey]);
            Assert.AreEqual(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.AreEqual(exception.ProblemDetails.Title, exception.Message);
            Assert.AreEqual(headers, exception.Headers);
            Assert.AreEqual(innerException, exception.InnerException);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("  ")]
        public void Create_WithResponseString_ResponseStringIsEmpty_ExceptionHasMinimalProblemDetails(string responseString)
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ProblemDetails.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiException.Create(statusCode, headers, responseString, null, innerException);

            Assert.AreEqual($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.AreEqual(statusCode, exception.ProblemDetails.Status);
            Assert.AreEqual(operationId, exception.ProblemDetails.OperationId);
            Assert.AreEqual(0, exception.ProblemDetails.Extensions.Count);
            Assert.AreEqual(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.AreEqual(exception.ProblemDetails.Title, exception.Message);
            Assert.AreEqual(headers, exception.Headers);
            Assert.AreEqual(innerException, exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
        }

        [TestMethod]
        public void Create_WithResponseString_ResponseStringIsNotProblemDetails_ExceptionHasMinimalProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ProblemDetails.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };
            var responseString = JsonConvert.SerializeObject(new Exception("An error occured."));
            Exception innerException = new Exception("An error occurred.");

            ApiException exception = ApiException.Create(statusCode, headers, responseString, null, innerException);

            Assert.AreEqual($"HTTP status code {statusCode}.", exception.ProblemDetails.Title);
            Assert.AreEqual(statusCode, exception.ProblemDetails.Status);
            Assert.AreEqual(operationId, exception.ProblemDetails.OperationId);
            Assert.AreEqual(0, exception.ProblemDetails.Extensions.Count);
            Assert.AreEqual(exception.ProblemDetails.Status, exception.StatusCode);
            Assert.AreEqual(exception.ProblemDetails.Title, exception.Message);
            Assert.AreEqual(headers, exception.Headers);
            Assert.AreEqual(innerException, exception.InnerException);
            AssertNullProblemDetailsOptionalProperties(exception.ProblemDetails);
        }
    }
}
