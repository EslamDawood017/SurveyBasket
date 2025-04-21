namespace SurveyBasket.Api.Contract.Question;

public record QuestionRequist(
    string Content,
    List<string> Answers
);

