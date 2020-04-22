using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using VecompSoftware.DocSuite.SPID.DataProtection;
using VecompSoftware.DocSuite.SPID.Model.Auth;
using VecompSoftware.DocSuite.SPID.Model.SAML;
using VecompSoftware.DocSuite.SPID.Model.Tokens;
using VecompSoftware.DocSuite.SPID.Model.Tokens.Types;

namespace VecompSoftware.DocSuite.SPID.JWT
{
    public class JwtService : IJwtService
    {
        #region [ Fields ]
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly IDataProtectionService _dataProtectionService;
        private readonly IDistributedCache _memoryCache;
        private const string NOT_SPECIFIED_VALUE = "";
        private const int HOURS_TO_EXPIRE = 1;
        #endregion

        #region [ Properties ]
        private static long ToUnixDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);
        #endregion

        #region [ Constructor ]
        public JwtService(IOptions<JwtConfiguration> jwtConfiguration, IDataProtectionService dataProtectionService, IDistributedCache memoryCache)
        {
            _jwtConfiguration = jwtConfiguration?.Value;
            _dataProtectionService = dataProtectionService;
            _memoryCache = memoryCache;
        }
        #endregion

        #region [ Methods ]
        public TokenResult Create(SamlResponse samlResponse)
        {
            DateTime issued = DateTime.UtcNow;
            DateTime expire = DateTime.UtcNow.AddHours(HOURS_TO_EXPIRE);
            string claimId = Guid.NewGuid().ToString();

            ICollection<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, samlResponse.User.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, claimId),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixDate(issued).ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, ToUnixDate(expire).ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, samlResponse.User.IdpReferenceId ?? NOT_SPECIFIED_VALUE),
                new Claim(JwtRegisteredClaimNames.Email, samlResponse.User.Email ?? NOT_SPECIFIED_VALUE),
                new Claim(ClaimDefinitions.PEC_CLAIM_NAME, samlResponse.User.PEC ?? NOT_SPECIFIED_VALUE),
                new Claim(ClaimDefinitions.MOBILE_PHONE_CLAIM_NAME, _dataProtectionService.Protect(samlResponse.User.MobilePhone ?? NOT_SPECIFIED_VALUE)),
                new Claim(ClaimDefinitions.ADDRESS_CLAIM_NAME, _dataProtectionService.Protect(samlResponse.User.Address ?? NOT_SPECIFIED_VALUE)),
                new Claim(JwtRegisteredClaimNames.GivenName, samlResponse.User.Name),
                new Claim(JwtRegisteredClaimNames.FamilyName, samlResponse.User.Surname),
                new Claim(JwtRegisteredClaimNames.Birthdate, samlResponse.User.DateOfBirth ?? NOT_SPECIFIED_VALUE),
                new Claim(JwtRegisteredClaimNames.Gender, samlResponse.User.Gender ?? NOT_SPECIFIED_VALUE),
                new Claim(ClaimDefinitions.FISCAL_NUMBER_CLAIM_NAME, samlResponse.User.FiscalNumber ?? NOT_SPECIFIED_VALUE),
                new Claim(ClaimDefinitions.PLACE_BIRTH_CLAIM_NAME, samlResponse.User.PlaceOfBirth ?? NOT_SPECIFIED_VALUE),
                new Claim(ClaimDefinitions.COMPANY_NAME_CLAIM_NAME, samlResponse.User.CompanyName ?? NOT_SPECIFIED_VALUE),
                new Claim(ClaimDefinitions.REGISTERED_OFFICE_CLAIM_NAME, _dataProtectionService.Protect(samlResponse.User.RegisteredOffice ?? NOT_SPECIFIED_VALUE)),
                new Claim(ClaimDefinitions.IVA_CODE_CLAIM_NAME, samlResponse.User.IvaCode ?? NOT_SPECIFIED_VALUE)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _jwtConfiguration.Issuer,
                audience: _jwtConfiguration.Issuer,
                claims: claims,
                signingCredentials: _jwtConfiguration.SigningCredentials);

            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            //Save to cache. TODO: persist?
            string sharedEncryptedKey = _dataProtectionService.Protect(claimId);
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(expire.AddMinutes(10));
            _memoryCache.Set(claimId, Encoding.UTF8.GetBytes(encodedJwt), cacheEntryOptions);

            return new TokenResult()
            {
                ReferenceCode = sharedEncryptedKey,
                Token = new JwtToken(encodedJwt)
            };
        }

        public string Find(string referenceCode)
        {
            return _memoryCache.GetString(referenceCode);
        }

        public string Refresh(string referenceCode, string expiredToken)
        {
            string claimId = _dataProtectionService.Unprotect(referenceCode);
            string token = _memoryCache.GetString(claimId);
            if (!string.IsNullOrEmpty(token))
            {
                DateTime expire = DateTime.UtcNow.AddHours(HOURS_TO_EXPIRE);
                JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(token);
                ICollection<Claim> clonedClaims = jwtSecurityToken.Claims.Where(x => x.Type != JwtRegisteredClaimNames.Exp).ToList();
                clonedClaims.Add(new Claim(JwtRegisteredClaimNames.Exp, ToUnixDate(expire).ToString()));
                JwtSecurityToken newToken = new JwtSecurityToken(
                    issuer: _jwtConfiguration.Issuer,
                    audience: _jwtConfiguration.Issuer,
                    claims: clonedClaims,
                    signingCredentials: _jwtConfiguration.SigningCredentials);

                string encodedJwt = new JwtSecurityTokenHandler().WriteToken(newToken);
                //Save to cache. TODO: persist?
                DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(expire.AddMinutes(10));
                _memoryCache.Set(claimId, Encoding.UTF8.GetBytes(encodedJwt), cacheEntryOptions);

                return encodedJwt;
            }
            return null;
        }

        public string Clone(string referenceCode, bool unprotectData)
        {
            string claimId = _dataProtectionService.Unprotect(referenceCode);
            string token = _memoryCache.GetString(claimId);
            if (!string.IsNullOrEmpty(token))
            {
                JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(token);
                IEnumerable<Claim> clonedClaims = jwtSecurityToken.Claims.Where(x => x.Type != JwtRegisteredClaimNames.Sub
                    && x.Type != JwtRegisteredClaimNames.Jti
                    && x.Type != JwtRegisteredClaimNames.Iat
                    && x.Type != JwtRegisteredClaimNames.Exp);
                if (unprotectData)
                {
                    clonedClaims = clonedClaims.Where(x => x.Type == ClaimDefinitions.REGISTERED_OFFICE_CLAIM_NAME
                                                        || x.Type == ClaimDefinitions.MOBILE_PHONE_CLAIM_NAME
                                                        || x.Type == ClaimDefinitions.ADDRESS_CLAIM_NAME)
                                               .Select(s => new Claim(s.Type, _dataProtectionService.Unprotect(s.Value))).Union(clonedClaims.Where(x => x.Type != ClaimDefinitions.REGISTERED_OFFICE_CLAIM_NAME
                                                        && x.Type != ClaimDefinitions.MOBILE_PHONE_CLAIM_NAME
                                                        && x.Type != ClaimDefinitions.ADDRESS_CLAIM_NAME));
                }

                JwtSecurityToken newToken = new JwtSecurityToken(
                    claims: clonedClaims.ToList(),
                    signingCredentials: jwtSecurityToken.SigningCredentials);

                return new JwtSecurityTokenHandler().WriteToken(newToken);
            }
            return null;
        }

        public void Remove(string referenceCode)
        {
            string claimId = _dataProtectionService.Unprotect(referenceCode);
            string token = _memoryCache.GetString(claimId);
            if (!string.IsNullOrEmpty(token))
            {
                _memoryCache.Remove(claimId);
            }
        }
        #endregion
    }
}
