namespace SurveyBasket.Api.Interfaces;

public interface INotificationService
{
    public Task SendNewPollsNotification(int? pollId = null);
}
