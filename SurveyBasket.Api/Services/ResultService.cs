using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Question;
using SurveyBasket.Api.Contract.Result;
using SurveyBasket.Api.Data;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Interfaces;

namespace SurveyBasket.Api.Services;

public class ResultService( AppDbContext context) : IResultService
{
    private readonly AppDbContext _context = context;

    public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken)
    {
        PollVotesResponse? result = await _context.Polls
            .Where(p => p.Id == pollId)
            .Select(p => new PollVotesResponse
            (
               p.Title,
               p.Votes.Select(v => new VoteReponse
               (
                   $"{v.ApplicationUser.FirstName} {v.ApplicationUser.LastName}",
                   v.SubmittedOn,
                   v.VoteAnswers.Select(a => new QuestionAnswerRespone
                   (
                       a.Question.Content,
                       a.Answer.Content
                   ))
               ))
            ))
            .SingleOrDefaultAsync(cancellationToken);

        return result is null
            ? Result.Failure<PollVotesResponse>(PollError.PollNotFound)
            : Result.Success(result);
            
               
    }

    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesCountPerDayAsync(int pollId, CancellationToken cancellationToken)
    {
        var isPollExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!isPollExist)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollError.PollNotFound);

        var VotesPerDay = await _context.Votes
            .Where(p => p.PollId == pollId)
            .GroupBy(p => new { Date = DateOnly.FromDateTime( p.SubmittedOn )})
            .Select(p=> new VotesPerDayResponse(
                p.Key.Date, 
                p.Count()
                ))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerDayResponse>>(VotesPerDay);
            
    }
    public async Task<Result<IEnumerable<VotePerQuestionResponse>>> GetVotesCountPerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var isPollExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!isPollExist)
            return Result.Failure<IEnumerable<VotePerQuestionResponse>>(PollError.PollNotFound);

        var votesPerQuestion = await _context.VoteAnswers
             .Where(a => a.Vote.PollId == pollId)
             .Select(a => new VotePerQuestionResponse(
                 a.Question.Content,
                 a.Question.Votes.GroupBy(x => new
                 {
                     Answers = a.Answer.Id, AnswerContent = x.Answer.Content
                 }).Select(g => new VotesPerAnswerResponse(
                     g.Key.AnswerContent,
                     g.Count()))
                 ))
             .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotePerQuestionResponse>>(votesPerQuestion);

    }
} 
