using Laserfiche.Api.Client.OAuth;
using Laserfiche.Api.Client.Utils;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Laserfiche.Api.Client.UnitTest
{
    [TestClass]
    public class JwtUtilsTests
    {
        private const string ServicePrincipalKey = "ServicePrincipalKey";
        private readonly AccessKey AccessKey = new AccessKey()
        {
            ClientId = "ClientId",
            Jwk = new Microsoft.IdentityModel.Tokens.JsonWebKey() 
            {
                Kty = "EC",
                Crv = "P-256",
                Use = "sig",
                Kid = "TqlmmB_nwSb6Yyov9qIcJVCLdBAGhonC7C7s9kC4Avs",
                X = "jdYj973SLwMIiuwA24TNXs1NmkvLeSzw-QBd_-_4-R8",
                Y = "wo3hyow9__af_4dIxsiL7Zs8oa2z4BTdS9LmX71Xj3w",
                D = "qEnaazhXsBpePbV8MYLGz8NUnt4CKW7p0utPIj_NR2k"
            }
        };

        [TestMethod]
        public void CreateClientCredentialsAuthorizationJwt_WithDefaultExpiration()
        {
            string result = JwtUtils.CreateClientCredentialsAuthorizationJwt(ServicePrincipalKey, AccessKey);

            var jwt = new JsonWebToken(result);
            Assert.AreEqual(AccessKey.Jwk.Kid, jwt.Kid);
            Assert.IsTrue(jwt.ValidTo > DateTime.UtcNow);
        }

        [TestMethod]
        public void CreateClientCredentialsAuthorizationJwt_WithCustomExpiration()
        {
            DateTime expiration = DateTime.UtcNow.AddHours(2);
            string result = JwtUtils.CreateClientCredentialsAuthorizationJwt(ServicePrincipalKey, AccessKey, expiration);

            var jwt = new JsonWebToken(result);
            Assert.AreEqual(AccessKey.Jwk.Kid, jwt.Kid);
            Assert.IsTrue(expiration.Subtract(jwt.ValidTo).Duration().TotalSeconds < 1);
        }

        [TestMethod]
        public void CreateClientCredentialsAuthorizationJwt_WithNullExpiration()
        {
            string result = JwtUtils.CreateClientCredentialsAuthorizationJwt(ServicePrincipalKey, AccessKey, null);

            var jwt = new JsonWebToken(result);
            Assert.AreEqual(AccessKey.Jwk.Kid, jwt.Kid);
            Assert.AreEqual(DateTime.MinValue, jwt.ValidTo);
        }
    }
}
