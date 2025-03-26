using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Api.Authentications;

public class JwtProvider : IJwtProvider
{
    public (string Token, int ExpiredIn) GenerateToken(ApplicationUser user)
    {
        
        Claim[] claims =  [
            new (JwtRegisteredClaimNames.Sub ,user.Id.ToString() ), 
            new (JwtRegisteredClaimNames.Email ,user.Email! ), 
            new (JwtRegisteredClaimNames.GivenName ,user.FirstName ), 
            new (JwtRegisteredClaimNames.FamilyName ,user.LastName), 
            new (JwtRegisteredClaimNames.Jti ,Guid.NewGuid().ToString() ),
            ];

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("UbRHQ0TEUHszoa9BQ3EeDUCFW0DMPAhM"));
    
        var signingCredentials = new SigningCredentials(symmetricSecurityKey , SecurityAlgorithms.HmacSha256);

        var expireIn = 30 ; 
        
        var token = new JwtSecurityToken(
            issuer: "SurveyBasketApp",
            audience: "SurveyBasketApp Users",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireIn),
            signingCredentials: signingCredentials);

        return (token: new JwtSecurityTokenHandler().WriteToken(token), expireIn * 60 );
    }
}
