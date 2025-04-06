namespace SurveyBasket.Api.Abstractions.Consts;

public class RegexPattern
{
    public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$";
}
