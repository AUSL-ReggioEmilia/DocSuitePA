using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using VecompSoftware.DocSuite.SPID.Model.Auth;

namespace VecompSoftware.DocSuite.SPID.AccessoAtti.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CivicNumber { get; set; }
        public string Email { get; set; }
        public string PEC { get; set; }

        public UserModel()
        {

        }

        public UserModel(JwtSecurityToken jwtSecurityToken)
        {
            DateTime? birthDate = null;
            if (DateTime.TryParse(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Birthdate)?.Value, out DateTime tmpDate))
            {
                birthDate = tmpDate;
            }
            Id = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId)?.Value;
            Name = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.GivenName)?.Value;
            Surname = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.FamilyName)?.Value;
            DateOfBirth = birthDate;
            PlaceOfBirth = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == ClaimDefinitions.PLACE_BIRTH_CLAIM_NAME)?.Value;
            Address = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == ClaimDefinitions.ADDRESS_CLAIM_NAME)?.Value;
            Email = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
        }
    }
}
