// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using Laserfiche.Api.Client.OAuth;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Laserfiche.Api.Client.Utils
{
    public static class JwtUtils
    {
        private const string AudienceClaim = "laserfiche.com";

        /// <summary>
        /// Create OAuth 2.0 client_credentials Authorization JWT that can be used with Laserfiche Cloud Token endpoint to request an Access Token.
        /// The Authorization JWT will expire after 30 minutes.
        /// </summary>
        /// <param name="servicePrincipalKey">The service principal key created for the service principal from the Laserfiche Account Administration.</param>
        /// <param name="accessKey">AccessKey exported from the Laserfiche Developer Console.</param>
        /// <returns>Authorization JWT.</returns>
        public static string CreateClientCredentialsAuthorizationJwt(string servicePrincipalKey, AccessKey accessKey)
        {
            return CreateClientCredentialsAuthorizationJwt(servicePrincipalKey, accessKey, DateTime.UtcNow.AddMinutes(30));
        }

        /// <summary>
        /// Create OAuth 2.0 client_credentials Authorization JWT that can be used with Laserfiche Cloud Token endpoint to request an Access Token.
        /// </summary>
        /// <param name="servicePrincipalKey">The service principal key created for the service principal from the Laserfiche Account Administration.</param>
        /// <param name="accessKey">AccessKey exported from the Laserfiche Developer Console.</param>
        /// <param name="validTo">The expiration time in UTC for when the authorization JWT expires. Set to null if the JWT never expires.</param>
        /// <returns>Authorization JWT.</returns>
        public static string CreateClientCredentialsAuthorizationJwt(string servicePrincipalKey, AccessKey accessKey, DateTime? validTo)
        {
            var claims = new[]
                {
                    new Claim("client_id", accessKey.ClientId),
                    new Claim("client_secret", servicePrincipalKey),
                };
            return CreateSignedJwt(claims, accessKey.Jwk, validTo);
        }

        private static SigningCredentials GetSigningCredentials(JsonWebKey key)
        {
            var ecdsa = ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = new ECPoint { X = Base64UrlEncoder.DecodeBytes(key.X), Y = Base64UrlEncoder.DecodeBytes(key.Y) },
                D = Base64UrlEncoder.DecodeBytes(key.D)
            });
            var ecdsaSecurityKey = new ECDsaSecurityKey(ecdsa) { KeyId = key.Kid };
            return new SigningCredentials(ecdsaSecurityKey, SecurityAlgorithms.EcdsaSha256);
        }

        private static string CreateSignedJwt(IEnumerable<Claim> claims, JsonWebKey key, DateTime? validTo = null)
        {
            var signingCredentials = GetSigningCredentials(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = AudienceClaim,
                Expires = validTo,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredentials,
                NotBefore = DateTime.UtcNow,
                IssuedAt = DateTime.UtcNow,
            };
            var tokenHandler = new JsonWebTokenHandler() { SetDefaultTimesOnTokenCreation = false };
            return tokenHandler.CreateToken(tokenDescriptor);
        }

        internal static string CreateBasicAuth(string clientId, string clientSecret)
        {
            if (clientSecret != null)
            {
                var basicCredentials = clientId + ':' + clientSecret;
                var base64EncodedClientSecret = System.Text.Encoding.UTF8.GetBytes(basicCredentials);
                return $"Basic {System.Convert.ToBase64String(base64EncodedClientSecret)}";
            }
            return null;
        }
    }
}
