using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.User;

namespace SurveyBasket.Api.Interfaces;

public interface IUserService
{
    public Task<Result> UpdateAsync(UpdateUserRequist requist, CancellationToken cancellationToken);
    public Task<Result<UserResponse>> GetAsync(int id);
    public Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<Result<UserProfileResponse>> UserProfileAsync(int userId);
    public Task<Result> UpdateUserInfoAsync(int userId, UpdateUserProfileRequist requist);
    public Task<Result> ChangePassword(int userId, ChangePasswordRequist requist);
    public Task<Result<UserResponse>> AddAsync(CreateUserRequist requist, CancellationToken cancellationToken);
    public Task<Result> ToggleStatusAsync(int UserId, CancellationToken cancellationToken);
    public Task<Result> Unlock(int UserId, CancellationToken cancellationToken);
}
