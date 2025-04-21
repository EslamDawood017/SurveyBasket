namespace SurveyBasket.Api.Consts;

public static class Permissions
{
    public static string Type { get; } = "Permissions";

    public const string GetPolls = "polls:read";
    public const string AddPolls = "polls:add";
    public const string UpdatePolls = "polls:update";
    public const string DeletePolls = "polls:delete";

    public const string GetQuestions = "Questions:read";
    public const string AddQuestions = "Questions:add";
    public const string UpdateQuestions = "Questions:update";

    public const string GetUser = "Users:read";
    public const string AddUser = "Users:add";
    public const string UpdateUser = "Users:update";

    public const string GetRole = "Roles:read";
    public const string AddRole = "Roles:add";
    public const string UpdateRole = "Roles:update";

    public const string Results = "Results:read";

    public static List<string?> GetAllPermissions()
    {
        return typeof(Permissions)
            .GetFields()
            .Select(f => f.GetValue(f) as string)
            .ToList();

    }
}
