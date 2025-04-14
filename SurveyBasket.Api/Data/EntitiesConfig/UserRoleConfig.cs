using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Consts;

namespace SurveyBasket.Api.Data.EntitiesConfig;

public class UserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<int>> builder)
    {
        builder.HasData(new IdentityUserRole<int>
        {
            RoleId = DefaultRoles.AdminRoleId,
            UserId = DefaultUsers.AdminId,
        });
    }
}
