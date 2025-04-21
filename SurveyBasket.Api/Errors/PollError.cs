using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Errors;


public class PollError
{
    public static readonly Error PollNotFound = new Error("Poll.NotFound", "No Poll Found With the given Id", StatusCodes.Status404NotFound);
    public static readonly Error EmptyList = new Error("Poll.EmptyList", "No Polls Found", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedPollTitle = new Error("Poll.Duplicated_Poll_Title", "Another poll with the same tilte exists", StatusCodes.Status409Conflict);


}
