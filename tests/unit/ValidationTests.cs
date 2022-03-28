using Laserfiche.Oauth.Api.Client;
using I18n = Laserfiche.Oauth.Api.Client.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Laserfiche.OAuth.Client.ClientCredentials.UnitTest
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void AllFieldsNeedToBeFilled()
        {
            IClientCredentialsOptions config = new ClientCredentialsOptions();
            var (isValid, reasons) = config.IsValid();
            Assert.IsFalse(isValid);
            Assert.AreEqual(4, reasons.Count);
            Assert.AreEqual(reasons[0], I18n.Strings.INVALID_BASE_ADDRESS);
            Assert.AreEqual(reasons[1], I18n.Strings.INVALID_CLIENT_ID);
            Assert.AreEqual(reasons[2], I18n.Strings.INVALID_SERVICE_PRINCIPAL_KEY);
            Assert.AreEqual(reasons[3], I18n.Strings.INVALID_SIGNING_KEY);
        }

        [TestMethod]
        public void HandlerConfigurationCannotBeNull()
        {
            var ex = Assert.ThrowsException<ArgumentNullException>(() => new ClientCredentialsHandler(null));
            Assert.AreEqual(ex.Message, new ArgumentNullException("configuration").Message);
        }
    }
}
