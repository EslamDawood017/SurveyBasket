using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Errors;

public class QuestionError
{
    public static readonly Error QuestionNotFound = new Error("Question.NotFound", "No Question Found With the given Id" , StatusCodes.Status404NotFound);
    public static readonly Error EmptyList = new Error("Question.EmptyList", "No Question Found" , StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedQuestionContent = new Error("Question.Duplicated_Question_Content", "Another Question with the same Content exists" , StatusCodes.Status409Conflict);
}
