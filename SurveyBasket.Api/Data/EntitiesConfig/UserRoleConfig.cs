using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Api.Data.EntitiesConfig;

public class UserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<int>> builder)
    {
        builder.HasData(new IdentityUserRole<int>
        {
            RoleId = DefaultRoles.Admin.Id,
            UserId = DefaultUsers.AdminId,
        });
    }
}
