using Mapster;
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.User;
using SurveyBasket.Api.Interfaces;

namespace SurveyBasket.Api.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result> ChangePassword(int userId, ChangePasswordRequist requist)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        var result = await _userManager.ChangePasswordAsync(user!, requist.CurrentPassword, requist.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.FirstOrDefault();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateUserInfoAsync(int userId, UpdateUserProfileRequist requist)
    {
        //var user = await _userManager.FindByIdAsync(userId.ToString());

        //user.FirstName = requist.FirstName;
        //user.LastName = requist.LastName;

        //await _userManager.UpdateAsync(user);
        await _userManager.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(setters =>
                setters
                .SetProperty(x => x.FirstName, requist.FirstName)
                .SetProperty(x => x.LastName, requist.LastName)
            );

        return Result.Success();
    }

    public async Task<Result<UserProfileResponse>> UserProfileAsync(int userId)
    {
        var user = await _userManager.Users
            .Where(x=>x.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .SingleOrDefaultAsync();

        return Result.Success(user)!;
    }
}
