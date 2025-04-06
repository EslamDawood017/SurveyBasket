using Mapster;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contract.Answer;
using SurveyBasket.Api.Contract.Question;
using SurveyBasket.Api.Data;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace SurveyBasket.Api.Services;

public class QuestionService(AppDbContext context , IDistributedCashService distributedCashService) : IQuestionService
{
    private readonly AppDbContext _context = context;
    private readonly IDistributedCashService _distributedCashService = distributedCashService;
    private const string _cashPrefix = "availableQuestion";
    public async Task<Result<QuestionResponse>> AddAsync(int PollId, QuestionRequist requist, CancellationToken cancellationToken)
    {
        var isPollExist = await _context.Polls.AnyAsync(p => p.Id == PollId , cancellationToken);

        if(!isPollExist)
            return Result.Failure<QuestionResponse>(PollError.PollNotFound);

        var isConentExist = await _context.Questions.AnyAsync(x => x.Content == requist.Content && x.PollId == PollId , cancellationToken);

        if(isConentExist)
            return Result.Failure<QuestionResponse>(QuestionError.DuplicatedQuestionContent);

        var question = requist.Adapt<Question>();

        question.PollId = PollId;


        await _context.Questions.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _distributedCashService.RemoveAsunc($"{_cashPrefix}-{PollId}");

        return Result.Success(question.Adapt<QuestionResponse>());  
    }
    public async Task<Result<ICollection<QuestionResponse>>> GetAllAsync(int PollId, CancellationToken cancellationToken)
    {
        var isPollExist = await _context.Polls.AnyAsync(p => p.Id == PollId, cancellationToken);

        if (!isPollExist)
            return Result.Failure <ICollection<QuestionResponse>> (PollError.PollNotFound);

        var PollQuestions = await _context.Questions
            .Where(x => x.PollId == PollId)
            .Include(x => x.Answers)
            //.Select(x => new QuestionResponse(
            //    x.Id,
            //    x.Content ,
            //    x.Answers.Select(a => new AnswerResponse(a.Id , a.Content ))))
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (!PollQuestions.Any())
            return Result.Failure<ICollection<QuestionResponse>>(QuestionError.EmptyList);

        return Result.Success<ICollection<QuestionResponse>>(PollQuestions);
    }

    public async Task<Result<QuestionResponse>> GetAsync(int PollId, int QuestionId ,CancellationToken cancellationToken)
    {
        var isPollExist = await _context.Polls.AnyAsync(p => p.Id == PollId, cancellationToken);

        if (!isPollExist)
            return Result.Failure<QuestionResponse>(PollError.PollNotFound);

        var isQuestionExist = await _context.Questions.AnyAsync(x => x.Id ==  QuestionId , cancellationToken);

        if(!isQuestionExist)
            return Result.Failure<QuestionResponse>(QuestionError.QuestionNotFound);

        var Question = await _context.Questions
            .Include(x => x.Answers)
            .SingleOrDefaultAsync(x => x.Id == QuestionId && x.PollId == PollId , cancellationToken);

        return Result.Success(Question.Adapt<QuestionResponse>());
       
    }
    public async Task<Result<ICollection<QuestionResponse>>> GetAvailableAsync(int PollId, int UserId, CancellationToken cancellationToken)
    {
        var hasVoted = await _context.Votes.AnyAsync(v => v.PollId == PollId && v.ApplicationUserId == UserId , cancellationToken);

        if (hasVoted)
            return Result.Failure<ICollection<QuestionResponse>>(VoteError.DuplicatedVote);

        var isPollExist = await _context.Polls.AnyAsync( p => p.Id == PollId && p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow));
      
        if (!isPollExist)
            return Result.Failure<ICollection<QuestionResponse>>(PollError.PollNotFound);

        var cashKey = $"{_cashPrefix}-{PollId}";

        var cashQuestions = await _distributedCashService.GetAsunc<ICollection<QuestionResponse>>(cashKey, cancellationToken);

        ICollection<QuestionResponse> questions = [];

        if(cashQuestions is null)
        {
            Console.WriteLine("Result From DataBase");
            questions = await _context.Questions
                .Where(x => x.PollId == PollId && x.IsActive)
                .Include(x => x.Answers)
                .Select(q => new QuestionResponse(
                    q.Id,
                    q.Content,
                    q.Answers.Select(a => new AnswerResponse(
                        a.Id,
                        a.Content))
                    ))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            await _distributedCashService.SetAsunc(cashKey, questions, cancellationToken);
        }
        else
        {
            Console.WriteLine("Result From Cash");
            questions = cashQuestions;
        }


        //var question = await _context.Questions
        //    .Where(x => x.PollId == PollId && x.IsActive)
        //    .Include(x => x.Answers)
        //    .Select(q => new QuestionResponse (
        //        q.Id ,
        //        q.Content ,
        //        q.Answers.Select(a => new AnswerResponse(
        //            a.Id ,
        //            a.Content))
        //        ))
        //    .AsNoTracking()
        //    .ToListAsync(cancellationToken);

        return Result.Success<ICollection<QuestionResponse>>(questions);
    
    }
    public async Task<Result> ToggleStatusAsync(int PollId, int QuestionId, CancellationToken cancellationToken)
    {

        var isPollExist = await _context.Polls.AnyAsync(p => p.Id == PollId, cancellationToken);

        if (!isPollExist)
            return Result.Failure<QuestionResponse>(PollError.PollNotFound);

        var isQuestionExist = await _context.Questions.AnyAsync(x => x.Id == QuestionId, cancellationToken);

        if (!isQuestionExist)
            return Result.Failure<QuestionResponse>(QuestionError.QuestionNotFound);

        var Question = await _context.Questions
            .Include(x => x.Answers)
            .SingleOrDefaultAsync(x => x.Id == QuestionId && x.PollId == PollId, cancellationToken);


        Question.IsActive = !Question.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        await _distributedCashService.RemoveAsunc($"{_cashPrefix}-{PollId}");

        return Result.Success();
    }
    public async Task<Result> UpdateAsync(int PollId, int QuestionId, QuestionRequist requist, CancellationToken cancellationToken)
    {

        var isPollExist = await _context.Polls.AnyAsync(p => p.Id == PollId, cancellationToken);

        if (!isPollExist)
            return Result.Failure<QuestionResponse>(PollError.PollNotFound);

        var isQuestionExist = await _context.Questions.AnyAsync(x => x.Id == QuestionId, cancellationToken);

        if (!isQuestionExist)
            return Result.Failure<QuestionResponse>(QuestionError.QuestionNotFound);

        var isQuestioncontentExist = await _context.Questions
            .AnyAsync(q =>
                q.PollId == PollId &&
                q.Id != QuestionId &&
                q.Content == requist.Content,
                cancellationToken);

        if(isQuestioncontentExist)
            return Result.Failure(QuestionError.DuplicatedQuestionContent);

        var question = await _context.Questions
            .Include(x => x.Answers)
            .SingleOrDefaultAsync(q => q.Id == QuestionId && q.PollId == PollId , cancellationToken);

        question.Content = requist.Content;

        // select answers 
        var currentAnswer = question.Answers.Select(a => a.Content).ToList();

        // add new answer 
        var newAnswers = requist.Answers.Except(currentAnswer).ToList();

        foreach(var answer in newAnswers)
        {
            question.Answers.Add(new Answer { Content = answer });
        }

        foreach(var answer in question.Answers.ToList())
        {
            answer.IsActive = requist.Answers.Contains(answer.Content);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _distributedCashService.RemoveAsunc($"{_cashPrefix}-{PollId}");

        return Result.Success();

    }
}
