namespace SurveyBasket.Api.Contract.Result;

public record PollVotesResponse(
    string title , 
    IEnumerable<VoteReponse> Votes);

