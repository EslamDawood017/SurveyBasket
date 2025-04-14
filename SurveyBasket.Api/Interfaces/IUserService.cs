using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.User;

namespace SurveyBasket.Api.Interfaces;

public interface IUserService
{
    public Task<Result<UserProfileResponse>> UserProfileAsync(int userId);
    public Task<Result> UpdateUserInfoAsync(int userId, UpdateUserProfileRequist requist);
    public Task<Result> ChangePassword(int userId, ChangePasswordRequist requist);
}
