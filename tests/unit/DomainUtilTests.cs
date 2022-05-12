using Microsoft.VisualStudio.TestTools.UnitTesting;
using Laserfiche.Api.Client.Util;

namespace Laserfiche.OAuth.Client.ClientCredentials.UnitTest
{
    [TestClass]
    public class DomainUtilTest
    {
        [TestMethod]
        public void GetDomainFromAccountId_US()
        {
            string accountId = "123123123";
            string domain = DomainUtil.GetDomainFromAccountId(accountId);
            Assert.AreEqual("laserfiche.com", domain);
        }

        [TestMethod]
        public void GetDomainFromAccountId_CA()
        {
            string accountId = "1111111111";
            string domain = DomainUtil.GetDomainFromAccountId(accountId);
            Assert.AreEqual("laserfiche.ca", domain);
        }

        [TestMethod]
        public void GetDomainFromAccountId_EU()
        {
            string accountId = "2222222222";
            string domain = DomainUtil.GetDomainFromAccountId(accountId);
            Assert.AreEqual("eu.laserfiche.com", domain);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("123")]
        [DataRow("3333333333")]
        [DataRow("123123123123")]
        public void GetDomainFromAccountId_DefaultUS(string accountId)
        {
            string domain = DomainUtil.GetDomainFromAccountId(accountId);
            Assert.AreEqual("laserfiche.com", domain);
        }

        [TestMethod]
        public void GetOauthBaseUri_Success()
        {
            string domain = "laserfiche.ca";
            string baseUri = DomainUtil.GetOauthBaseUri(domain);
            Assert.AreEqual($"https://signin.{domain}/oauth", baseUri);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void GetOauthBaseUri_DefaultUri(string domain)
        {
            string baseUri = DomainUtil.GetOauthBaseUri(domain);
            Assert.AreEqual($"https://signin.laserfiche.com/oauth", baseUri);
        }

        [TestMethod]
        public void GetOauthTokenUri_Success()
        {
            string domain = "laserfiche.ca";
            string baseUri = DomainUtil.GetOauthTokenUri(domain);
            Assert.AreEqual($"https://signin.{domain}/oauth/Token", baseUri);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void GetOauthTokenUri_DefaultUri(string domain)
        {
            string baseUri = DomainUtil.GetOauthTokenUri(domain);
            Assert.AreEqual($"https://signin.laserfiche.com/oauth/Token", baseUri);
        }
    }
}
