using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Laserfiche.Oauth.Api.Client.Util
{
    internal static class JwtUtil
    {
        internal static JsonWebToken ReadJWT(string jwt)
        {
            return new JsonWebTokenHandler().ReadJsonWebToken(jwt);
        }

        internal static string CreateClientCredentialsAuthorizationJwt(IClientCredentialsOptions config, string audience = "laserfiche.com", DateTime? validTo = null)
        {
            var claims = new[]
                {
                    new Claim("client_id", config.ClientId),
                    new Claim("client_secret", config.ServicePrincipalKey),
                };
            return CreateSignedJwt(claims, config.SigningKey, audience, validTo);
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

        private static string CreateSignedJwt(IEnumerable<Claim> claims, JsonWebKey key, string audience = "laserfiche.com",
            DateTime? validTo = null)
        {
            if(validTo == null)
                validTo = DateTime.UtcNow.AddMinutes(30);
            var signingCredentials = GetSigningCredentials(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = audience,
                Expires = validTo,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredentials
            };
            return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        }
    }
}
