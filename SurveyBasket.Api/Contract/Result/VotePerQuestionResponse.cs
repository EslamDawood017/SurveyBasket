namespace SurveyBasket.Api.Contract.Result;

public record VotePerQuestionResponse(string Question, IEnumerable<VotesPerAnswerResponse> SelectedAnswers);

