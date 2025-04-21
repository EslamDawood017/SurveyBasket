namespace SurveyBasket.Api.Contract.Result;

public record VoteReponse(
    string VoterName,
    DateTime VotingDate,
    IEnumerable<QuestionAnswerRespone> SelectedAnswers
);

