using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Result;

namespace SurveyBasket.Api.Interfaces;

public interface IResultService
{
    public Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken);
    public Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesCountPerDayAsync(int pollId, CancellationToken cancellationToken);
    public Task<Result<IEnumerable<VotePerQuestionResponse>>> GetVotesCountPerQuestionAsync(int pollId, CancellationToken cancellationToken);
}
