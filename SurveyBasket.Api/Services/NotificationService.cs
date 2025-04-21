using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasket.Api.Data;
using SurveyBasket.Api.Helpers;
using SurveyBasket.Api.Interfaces;

namespace SurveyBasket.Api.Services;

public class NotificationService(
    AppDbContext context,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender) : INotificationService
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;

    public async Task SendNewPollsNotification(int? pollId = null)
    {
        IEnumerable<Poll> polls = [];

        if (pollId.HasValue)
        {
            var poll = await _context.Polls.SingleOrDefaultAsync(p => p.IsPublished && p.Id == pollId);

            polls = [poll!];
        }
        else
        {
            polls = await _context.Polls
                .Where(p => p.IsPublished && p.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking()
                .ToArrayAsync();
        }

        //TODO Select User only
        var users = await _userManager.GetUsersInRoleAsync(DefaultRoles.Member.Name);

        var origin = _httpContextAccessor.HttpContext.Request.Headers.Origin;

        foreach (var poll in polls)
        {
            foreach (var user in users)
            {
                var placeholders = new Dictionary<string, string>
                {
                    {"{{name}}" , user.FirstName} ,
                    {"{{pollTill}}" , poll.Title} ,
                    {"{{endDate}}" , poll.EndsAt.ToString()} ,
                    {"{{url}}" ,  $"{origin}/polls/start/{poll.Id}"} ,
                };

                var body = EmailBodyBuilder.GenerateEmailBody("PollNotifications", placeholders);

                await _emailSender.SendEmailAsync(user.Email!, $"Survey Basket : New Poll - {poll.Title}", body);
            }
        }


    }
}
