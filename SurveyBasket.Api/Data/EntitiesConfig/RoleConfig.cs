namespace SurveyBasket.Api.Data.EntitiesConfig;

public class RoleConfig : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData(
            [
                new ApplicationRole
                {
                    Id = DefaultRoles.Admin.Id,
                    Name = DefaultRoles.Admin.Name,
                    NormalizedName = DefaultRoles.Admin.Name.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.Admin.ConcurrencyStamp,
                    IsDefault = false ,
                    IsDeleted = false
                },
                new ApplicationRole
                {
                    Id = DefaultRoles.Member.Id,
                    Name = DefaultRoles.Member.Name,
                    NormalizedName = DefaultRoles.Member.Name.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.Member.ConcurrencyStamp,
                    IsDefault = true ,
                    IsDeleted = false
                }
            ]
        );
    }


}
