



namespace SurveyBasket.Api.Data.EntitiesConfig;

public class VoteConfig : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasIndex(x => new {x.PollId , x.ApplicationUserId}).IsUnique();    
    }
}
