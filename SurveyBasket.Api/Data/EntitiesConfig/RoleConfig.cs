using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Consts;

namespace SurveyBasket.Api.Data.EntitiesConfig;

public class RoleConfig : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(
            [
                new ApplicationRole
                {
                    Id = DefaultRoles.AdminRoleId,
                    Name = DefaultRoles.Admin,
                    NormalizedName = DefaultRoles.Admin.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
                    IsDefault = false , 
                    IsDeleted = false 
                },
                new ApplicationRole
                {
                    Id = DefaultRoles.MemberRoleId,
                    Name = DefaultRoles.Member,
                    NormalizedName = DefaultRoles.Member.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.MemberRoleConcurrencyStamp,
                    IsDefault = true ,
                    IsDeleted = false
                }
            ]
        );
    }

    
}
