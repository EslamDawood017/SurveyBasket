

using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Poll;
using SurveyBasket.Api.Contract.Requist;

namespace SurveyBasket.Api.Interfaces;

public interface IPollService
{

    Task<Result<List<PollResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<List<PollResponse>>> GetCurrentAsync(CancellationToken cancellationToken = default);
    Task<Result<List<PollResponseV2>>> GetCurrentAsyncV2(CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> AddAsync(Poll poll, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int id, PollRequist poll, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken);
}
