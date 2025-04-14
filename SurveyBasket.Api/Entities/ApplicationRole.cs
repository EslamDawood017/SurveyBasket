using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Api.Entities;

public class ApplicationRole : IdentityRole<int>
{
    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }

}
