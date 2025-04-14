using Mapster;
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Roles;
using SurveyBasket.Api.Data;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Interfaces;

namespace SurveyBasket.Api.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager , AppDbContext context) : IRoleSerivce
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly AppDbContext _context = context;

    public async Task<Result> UpdateAsync(int id, RoleRequist request)
    {
        var roleIsExists = await _roleManager.Roles.AnyAsync(x => x.Name == request.Name && x.Id != id);

        if (roleIsExists)
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedRole);

        if (await _roleManager.FindByIdAsync(id.ToString()) is not { } role)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

        var allowedPermissions = Permissions.GetAllPermissions();

        if (request.Permission.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermission);

        role.Name = request.Name;

        var result = await _roleManager.UpdateAsync(role);

        if (result.Succeeded)
        {
            var currentPermissions = await _context.RoleClaims
                .Where(x => x.RoleId == id && x.ClaimType == Permissions.Type)
                .Select(x => x.ClaimValue)
                .ToListAsync();

            var newPermissions = request.Permission.Except(currentPermissions)
                .Select(x => new IdentityRoleClaim<int>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });

            var removedPermissions = currentPermissions.Except(request.Permission);

            await _context.RoleClaims
                .Where(x => x.RoleId == id && removedPermissions.Contains(x.ClaimValue))
            .ExecuteDeleteAsync();


            await _context.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        var error = result.Errors.First();

        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result<RoleDetailResponse>> AddAsync(RoleRequist requist)
    {
        var roleIsExist = await _roleManager.RoleExistsAsync(requist.Name);

        if (roleIsExist)
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedRole);

        var allowedPermissions = Permissions.GetAllPermissions();

        if(requist.Permission.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermission);

        var role = new ApplicationRole
        {
            Name = requist.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            IsDeleted = false,
            IsDefault = false,
            NormalizedName = requist.Name.ToUpper()
        };
        
        var result = await _roleManager.CreateAsync(role);

        if(result.Succeeded)
        {
            var permission = requist.Permission
                .Select(x => new IdentityRoleClaim<int>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id  
                });

            await _context.AddRangeAsync(permission);
            await _context.SaveChangesAsync();

            var response = new RoleDetailResponse(role.Id, role.Name, role.IsDeleted, requist.Permission);

            return Result.Success(response);
        }

        var error = result.Errors.FirstOrDefault();

        return Result.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }
    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool includeDeleted = false)
    {
        return await _roleManager.Roles
                .Where(x => !x.IsDefault && (includeDeleted || !x.IsDeleted))
                .ProjectToType<RoleResponse>()
                .ToListAsync();

    }
    public async Task<Result<RoleDetailResponse>> GetById(int Id)
    {
        var role = await _roleManager.FindByIdAsync(Id.ToString());

        if (role is null)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

        var permission = await _roleManager.GetClaimsAsync(role);

        var response = new RoleDetailResponse(role.Id , role.Name , role.IsDeleted , permission.Select(x => x.Value));

        return Result.Success(response);
    }
    public async Task<Result> ToggleStatusAsync(int roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());

        if (role is null)
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

        role.IsDeleted = !role.IsDeleted;

        await _roleManager.UpdateAsync(role);

        return Result.Success();
    }
}
