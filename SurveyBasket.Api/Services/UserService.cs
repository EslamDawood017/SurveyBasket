using Mapster;
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.User;
using SurveyBasket.Api.Data;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Interfaces;

namespace SurveyBasket.Api.Services;

public class UserService(
    UserManager<ApplicationUser> userManager, 
    AppDbContext appDbContext, 
    IRoleSerivce roleSerivce) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly AppDbContext _context = appDbContext;
    private readonly IRoleSerivce _roleSerivce = roleSerivce;

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
            .Where(x => x.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .SingleOrDefaultAsync();

        return Result.Success(user)!;
    }
    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await (from u in _context.Users
               join ur in _context.UserRoles
               on u.Id equals ur.UserId
               join r in _context.Roles
               on ur.RoleId equals r.Id into roles
               where !roles.Any(x => x.Name == DefaultRoles.Member)
               select new
               {
                   u.Id,
                   u.FirstName,
                   u.LastName,
                   u.Email,
                   u.IsActive,
                   Roles = roles.Select(x => x.Name!).ToList()
               }
                )
                .GroupBy(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsActive })
                .Select(u => new UserResponse
                (
                    u.Key.Id,
                    u.Key.FirstName,
                    u.Key.LastName,
                    u.Key.Email,
                    u.Key.IsActive,
                    u.SelectMany(x => x.Roles)
                ))
               .ToListAsync(cancellationToken);
    } 
    public async Task<Result<UserResponse>> GetAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        
        if (user is null)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        var userRoles = await _userManager.GetRolesAsync(user);

        var resuslt = new UserResponse
        (
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.IsActive,
            userRoles
        );

        return Result.Success<UserResponse>(resuslt);

    }
    public async Task<Result<UserResponse>> AddAsync(CreateUserRequist requist , CancellationToken cancellationToken)
    {
        var isEmailExist = await _userManager.Users.AnyAsync(x => x.Email == requist.Email, cancellationToken);

        if (isEmailExist)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        var user = requist.Adapt<ApplicationUser>();

        var allowedRoles = await _roleSerivce.GetAllAsync();

        if(requist.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        
        user.UserName = requist.Email;
        user.EmailConfirmed = true; 


        var result = await _userManager.CreateAsync(user , requist.Password);

        if(result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, requist.Roles);

            var resuslt = new UserResponse
           (
               user.Id,
               user.Email,
               user.FirstName,
               user.LastName,
               user.IsActive,
               requist.Roles
           );

            return Result.Success<UserResponse>(resuslt);
        }

        var error = result.Errors.FirstOrDefault();

        return Result.Failure<UserResponse>(new Error(error.Code , error.Description , StatusCodes.Status400BadRequest ));

    }
    public async Task<Result> UpdateAsync(UpdateUserRequist requist, CancellationToken cancellationToken)
    {
        var isEmailExist = await _userManager.Users.AnyAsync(x => x.Email == requist.Email && x.Id != requist.Id, cancellationToken);

        if (isEmailExist)
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = await _userManager.FindByIdAsync(requist.Id.ToString());

        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);


        user = requist.Adapt(user);     
     
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded )
        {
            await _context.UserRoles
                .Where(x => x.UserId == requist.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await _userManager.AddToRolesAsync(user, requist.Roles);

            var resuslt = new UserResponse
           (
               user.Id,
               user.Email!,
               user.FirstName,
               user.LastName,
               user.IsActive,
               requist.Roles
           );

            return Result.Success();
        }

        var error = result.Errors.FirstOrDefault();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }
    public async Task<Result> ToggleStatusAsync(int UserId, CancellationToken cancellationToken)
    {
       
        var user = await _userManager.FindByIdAsync(UserId.ToString());

        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);


        user.IsActive = !user.IsActive;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.FirstOrDefault();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }
    public async Task<Result> Unlock(int UserId, CancellationToken cancellationToken)
    {

        var user = await _userManager.FindByIdAsync(UserId.ToString());

        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);


        var result = await _userManager.SetLockoutEndDateAsync(user , null);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.FirstOrDefault();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }

}
