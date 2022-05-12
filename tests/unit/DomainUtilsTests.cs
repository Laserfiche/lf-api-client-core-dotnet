using Microsoft.VisualStudio.TestTools.UnitTesting;
using Laserfiche.Api.Client.Utils;
using System;

namespace Laserfiche.OAuth.Client.ClientCredentials.UnitTest
{
    [TestClass]
    public class DomainUtilTest
    {
        [TestMethod]
        public void GetDomainFromAccountId_US()
        {
            string accountId = "123123123";
            string domain = DomainUtils.GetDomainFromAccountId(accountId);
            Assert.AreEqual("laserfiche.com", domain);
        }

        [TestMethod]
        public void GetDomainFromAccountId_CA()
        {
            string accountId = "1111111111";
            string domain = DomainUtils.GetDomainFromAccountId(accountId);
            Assert.AreEqual("laserfiche.ca", domain);
        }

        [TestMethod]
        public void GetDomainFromAccountId_EU()
        {
            string accountId = "2222222222";
            string domain = DomainUtils.GetDomainFromAccountId(accountId);
            Assert.AreEqual("eu.laserfiche.com", domain);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [DataTestMethod]
        [DataRow("")]
        [DataRow("123")]
        [DataRow("3333333333")]
        [DataRow("123123123123")]
        public void GetDomainFromAccountId_IncorrectAccountId(string accountId)
        {
            DomainUtils.GetDomainFromAccountId(accountId);
        }

        [TestMethod]
        public void GetOauthBaseUri_Success()
        {
            string domain = "laserfiche.ca";
            string baseUri = DomainUtils.GetOAuthApiBaseUri(domain);
            Assert.AreEqual($"https://signin.{domain}/oauth/", baseUri);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void GetOauthBaseUri_NullEmptyDomain(string domain)
        {
            DomainUtils.GetOAuthApiBaseUri(domain);
        }

        [TestMethod]
        public void GetRepositoryApiBaseUri_Success()
        {
            string domain = "laserfiche.ca";
            string baseUri = DomainUtils.GetRepositoryApiBaseUri(domain);
            Assert.AreEqual($"https://api.{domain}/repository/", baseUri);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void GetRepositoryApiBaseUri_NullEmptyDomain(string domain)
        {
            DomainUtils.GetRepositoryApiBaseUri(domain);
        }
    }
}
