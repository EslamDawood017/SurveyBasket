using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Roles;

namespace SurveyBasket.Api.Interfaces;

public interface IRoleSerivce
{
    public Task<IEnumerable<RoleResponse>> GetAllAsync(bool includeDeleted = false);
    public Task<Result<RoleDetailResponse>> GetById(int Id);
    public Task<Result<RoleDetailResponse>> AddAsync(RoleRequist requist);
    public Task<Result> UpdateAsync(int roleId, RoleRequist requist);
    public Task<Result> ToggleStatusAsync(int roleId);

}
