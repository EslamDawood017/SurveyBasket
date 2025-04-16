using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Common;
using SurveyBasket.Api.Contract.Question;

namespace SurveyBasket.Api.Interfaces;

public interface IQuestionService
{
    public Task<Result<QuestionResponse>> AddAsync(int PollId , QuestionRequist requist , CancellationToken cancellationToken);
    public Task<Result<QuestionResponse>> GetAsync(int PollId, int QuestionId , CancellationToken cancellationToken);
    public Task<Result<PignatedList<QuestionResponse>>> GetAllAsync(int PollId ,RequestFilter requestFilter ,  CancellationToken cancellationToken);
    public Task<Result<ICollection<QuestionResponse>>> GetAvailableAsync(int PollId,int UserId , CancellationToken cancellationToken);
    public Task<Result> ToggleStatusAsync(int PollId, int QuestionId, CancellationToken cancellationToken);
    public Task<Result> UpdateAsync(int PollId, int QuestionId, QuestionRequist requist , CancellationToken cancellationToken);
}
