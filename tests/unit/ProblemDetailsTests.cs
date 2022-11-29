using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Laserfiche.Api.Client.UnitTest
{
    [TestClass]
    public class ProblemDetailsTests
    {
        [TestMethod]
        public void Deserialize_StringIsProblemDetailsWithExtensions()
        {
            var mockProblemDetails = new MockProblemDetails()
            {
                Title = "An error occurred.",
                Status = 400,
                Description = "a description extension property"
            };
            var serializedObject = JsonConvert.SerializeObject(mockProblemDetails);

            var result = JsonConvert.DeserializeObject<ProblemDetails>(serializedObject);

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
            var serializedObject = JsonConvert.SerializeObject(obj);

            Assert.ThrowsException<JsonSerializationException>(() => JsonConvert.DeserializeObject<ProblemDetails>(serializedObject));
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
