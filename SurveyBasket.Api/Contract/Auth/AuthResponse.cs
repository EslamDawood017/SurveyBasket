namespace SurveyBasket.Api.Contract.Auth;

public record AuthResponse (
    int Id ,
    string? Email ,
    string FirstName , 
    string LastName ,
    string Token , 
    int ExpiresIn,
    string ResfreshToken , 
    DateTime ResfreshTokenExpirations
);

