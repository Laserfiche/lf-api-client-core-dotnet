using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Laserfiche.Api.Client.UnitTest
{
    [TestClass]
    public class ProblemDetailsTests
    {
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings() { MaxDepth = 128 };

        [TestMethod]
        public void Create_ReturnMinimalProblemDetails()
        {
            int statusCode = 400;
            string operationId = "123456789";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                [ProblemDetails.OPERATION_ID_HEADER] = new List<string>() { operationId }
            };

            ProblemDetails result = ProblemDetails.Create(statusCode, headers);

            Assert.AreEqual($"HTTP status code {statusCode}.", result.Title);
            Assert.AreEqual(statusCode, result.Status);
            Assert.AreEqual(operationId, result.OperationId);
            Assert.IsNull(result.Type);
            Assert.IsNull(result.Instance);
            Assert.IsNull(result.Detail);
            Assert.IsNull(result.ErrorSource);
            Assert.IsNull(result.TraceId);
            Assert.AreEqual(default, result.ErrorCode);
            Assert.AreEqual(0, result.Extensions.Count);
        }

        [TestMethod]
        public void Create_NullHeaders_ReturnMinimalProblemDetails_NoOperationId()
        {
            int statusCode = 400;

            ProblemDetails result = ProblemDetails.Create(statusCode, null);

            Assert.AreEqual($"HTTP status code {statusCode}.", result.Title);
            Assert.AreEqual(statusCode, result.Status);
            Assert.IsNull(result.OperationId);
            Assert.IsNull(result.Type);
            Assert.IsNull(result.Instance);
            Assert.IsNull(result.Detail);
            Assert.IsNull(result.ErrorSource);
            Assert.IsNull(result.TraceId);
            Assert.AreEqual(default, result.ErrorCode);
            Assert.AreEqual(0, result.Extensions.Count);
        }

        [TestMethod]
        public void Deserialize_StringIsProblemDetailsWithExtensions()
        {
            var mockProblemDetails = new MockProblemDetails()
            {
                Title = "An error occurred.",
                Status = 400,
                Description = "a description extension property"
            };
            var serializedObject = JsonConvert.SerializeObject(mockProblemDetails, jsonSerializerSettings);

            var result = JsonConvert.DeserializeObject<ProblemDetails>(serializedObject, jsonSerializerSettings);

            Assert.AreEqual(mockProblemDetails.Title, result.Title);
            Assert.AreEqual(mockProblemDetails.Status, result.Status);
            Assert.AreEqual(1, result.Extensions.Count);
            Assert.AreEqual(mockProblemDetails.Description, result.Extensions[nameof(MockProblemDetails.Description)]);
        }

        [TestMethod]
        public void Deserialize_StringNotProblemDetails_ThrowsException()
        {
            var obj = new Student()
            {
                Id = 1,
                Name = "John",
            };
            var serializedObject = JsonConvert.SerializeObject(obj, jsonSerializerSettings);

            Assert.ThrowsException<JsonSerializationException>(() => JsonConvert.DeserializeObject<ProblemDetails>(serializedObject, jsonSerializerSettings));
        }

        private class Student
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class MockProblemDetails
        {
            public int Status { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
        }
    }
}
