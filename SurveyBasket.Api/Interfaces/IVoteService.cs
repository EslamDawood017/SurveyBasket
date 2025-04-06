using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Vote;

namespace SurveyBasket.Api.Interfaces;

public interface IVoteService
{
    public Task<Result> AddAsync(int pollId, int userId, VoteRequist voteRequist, CancellationToken cancellationToken = default!);
}
