namespace SurveyBasket.Api.Consts;

public class DefaultRoles
{
    public partial class Admin
    {
        public const int Id = 1;
        public const string Name = nameof(Admin);
        public const string ConcurrencyStamp = "7818D053-420B-4746-B538-303D3ECF0A71";
    }
    public partial class Member
    {
        public const int Id = 2;
        public const string Name = nameof(Member);
        public const string ConcurrencyStamp = "0632FD93-D499-45B6-8691-0768FE747546";
    }
}
