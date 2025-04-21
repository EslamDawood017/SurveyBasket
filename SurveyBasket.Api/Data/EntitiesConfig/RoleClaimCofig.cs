using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Api.Data.EntitiesConfig;

public class RoleClaimCofig : IEntityTypeConfiguration<IdentityRoleClaim<int>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<int>> builder)
    {
        var permissions = Permissions.GetAllPermissions();

        var adminClaims = new List<IdentityRoleClaim<int>>();

        for (int i = 0; i < permissions.Count; i++)
        {
            adminClaims.Add(new IdentityRoleClaim<int>
            {
                Id = i + 1,
                ClaimType = Permissions.Type,
                ClaimValue = permissions[i],
                RoleId = DefaultRoles.Admin.Id
            });
        }
        builder.HasData(adminClaims);
    }
}
