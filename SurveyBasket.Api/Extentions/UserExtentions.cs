using System.Security.Claims;

namespace SurveyBasket.Api.Extentions;

public static class UserExtentions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        return Convert.ToInt32( user.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
