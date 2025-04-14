namespace SurveyBasket.Api.Contract.User;

public record UserProfileResponse(
    string Email,
    string UserName ,
    string FirstName , 
    string LastName
);
