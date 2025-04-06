using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Errors;

public class VoteError
{
    public static Error DuplicatedVote = new Error(Code: "Vote.DuplicatedVote",
        "this person has already vote before for this poll" , StatusCodes.Status409Conflict);

    public static Error InvalidQuestions = new Error(Code: "Vote.InvalidQuestions",
        "Invalid questions" , StatusCodes.Status400BadRequest);
}