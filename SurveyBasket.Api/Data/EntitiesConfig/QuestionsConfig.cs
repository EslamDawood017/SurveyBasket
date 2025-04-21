



namespace SurveyBasket.Api.Data.EntitiesConfig;

public class QuestionsConfig : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasIndex(x => new { x.PollId, x.Content }).IsUnique();

        builder.Property(x => x.Content).HasMaxLength(1000);


    }
}
