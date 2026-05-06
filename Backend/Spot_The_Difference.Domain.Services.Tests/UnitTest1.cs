using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Spot_The_Difference.Contracts.Requests;
using Spot_The_Difference.Persistence.Entities.MijnMap;

namespace Spot_The_Difference.Domain.Services.Tests;

public class GameAndAdminServiceTests
{
    private static SpotthedifferencedbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SpotthedifferencedbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SpotthedifferencedbContext(options);
    }

    private static LevelService CreateLevelService(SpotthedifferencedbContext context)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost:5187");
        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        return new LevelService(context, accessor);
    }

    [Fact]
    public async Task ProcessGuessAsync_WhenHitInsideDifference_MarksRoundCompletedAndAwardsScore()
    {
        using var context = CreateContext();
        context.Playerrounds.Add(new Playerround
        {
            PlayerRoundId = 10,
            RoundId = 1,
            StartTime = DateTime.Now.AddSeconds(-25),
            Score = 0,
            Completed = false,
            CorrectDifferences = 0
        });
        context.Differences.Add(new Difference { RoundId = 1, X = 100, Y = 120, Width = 20, Height = 20 });
        await context.SaveChangesAsync();

        var service = new GameService(context, CreateLevelService(context));
        var response = await service.ProcessGuessAsync(new GuessRequest { PlayerRoundId = 10, X = 101, Y = 119 });

        Assert.NotNull(response);
        Assert.True(response!.IsCorrect);
        Assert.True(response.IsGameOver);

        var updatedRound = await context.Playerrounds.FirstAsync(pr => pr.PlayerRoundId == 10);
        Assert.True(updatedRound.Completed);
        Assert.Equal(100, updatedRound.Score);
        Assert.Equal(1, updatedRound.CorrectDifferences);
    }

    [Fact]
    public async Task ProcessGuessAsync_WhenMiss_DeductsScoreAndReturnsIncorrect()
    {
        using var context = CreateContext();
        context.Playerrounds.Add(new Playerround
        {
            PlayerRoundId = 11,
            RoundId = 2,
            Score = 30,
            Completed = false
        });
        context.Differences.Add(new Difference { RoundId = 2, X = 50, Y = 50, Width = 20, Height = 20 });
        await context.SaveChangesAsync();

        var service = new GameService(context, CreateLevelService(context));
        var response = await service.ProcessGuessAsync(new GuessRequest { PlayerRoundId = 11, X = 500, Y = 500 });

        Assert.NotNull(response);
        Assert.False(response!.IsCorrect);
        Assert.Equal("Mis!", response.Message);

        var updatedRound = await context.Playerrounds.FirstAsync(pr => pr.PlayerRoundId == 11);
        Assert.Equal(20, updatedRound.Score);
        Assert.False(updatedRound.Completed);
    }

    [Fact]
    public async Task ProcessGuessAsync_WhenRoundAlreadyCompleted_ReturnsAlreadyFound()
    {
        using var context = CreateContext();
        context.Playerrounds.Add(new Playerround
        {
            PlayerRoundId = 12,
            RoundId = 3,
            Score = 100,
            Completed = true
        });
        context.Differences.Add(new Difference { RoundId = 3, X = 80, Y = 80, Width = 20, Height = 20 });
        await context.SaveChangesAsync();

        var service = new GameService(context, CreateLevelService(context));
        var response = await service.ProcessGuessAsync(new GuessRequest { PlayerRoundId = 12, X = 80, Y = 80 });

        Assert.NotNull(response);
        Assert.True(response!.IsCorrect);
        Assert.True(response.IsAlreadyFound);
        Assert.True(response.IsGameOver);
    }

    [Fact]
    public async Task StartNewGameAsync_WhenRoundExists_ReturnsLocalizedQuestionAndCreatesPlayerRound()
    {
        using var context = CreateContext();
        var difficulty = new Difficulty { DifficultyId = 5, Name = "EASY", TimeLimitSeconds = 45 };
        var original = new Image { ImageId = 1, Path = "/images/original.jpg", Type = "original" };
        var difference = new Image { ImageId = 2, Path = "/images/difference.jpg", Type = "difference" };
        var round = new Round
        {
            RoundId = 20,
            DifficultyId = 5,
            Difficulty = difficulty,
            OriginalImageId = 1,
            OriginalImage = original,
            DifferenceImageId = 2,
            DifferenceImage = difference,
            Title = "Test Round"
        };
        var question = new Question { QuestionId = 1, RoundId = 20, Round = round, Text = "Question EN", TextNl = "Vraag NL" };
        question.Answeroptions.Add(new Answeroption { AnswerId = 100, QuestionId = 1, Question = question, Text = "Answer EN", TextNl = "Antwoord NL", IsCorrect = true });
        round.Questions.Add(question);

        context.Difficulties.Add(difficulty);
        context.Images.AddRange(original, difference);
        context.Rounds.Add(round);
        await context.SaveChangesAsync();

        var service = new GameService(context, CreateLevelService(context));
        var response = await service.StartNewGameAsync(new StartGameRequest
        {
            Username = "jonas",
            DifficultyId = 5,
            Language = "nl"
        });

        Assert.NotNull(response);
        Assert.Equal("Vraag NL", response!.QuestionText);
        Assert.Single(response.Answers);
        Assert.Equal("Antwoord NL", response.Answers[0].Text);
        Assert.Contains("/images/original.jpg", response.OriginalImageUrl);
        Assert.Contains("/images/difference.jpg", response.DifferenceImageUrl);

        Assert.Equal(1, await context.Players.CountAsync(p => p.Username == "jonas"));
        Assert.Equal(1, await context.Playerrounds.CountAsync(pr => pr.RoundId == 20));
    }

    [Fact]
    public async Task AdminService_CreateRoundAsync_CreatesRoundWithDifferencesAndAnswers()
    {
        using var context = CreateContext();
        context.Difficulties.Add(new Difficulty { DifficultyId = 1, Name = "EASY", TimeLimitSeconds = 60 });
        await context.SaveChangesAsync();

        var service = new AdminService(context);
        var request = new CreateRoundRequest
        {
            Name = "New round",
            Difficulty = "EASY",
            OriginalImage = "http://localhost/images/old.png",
            DifferenceImage = "http://localhost/images/new.png",
            QuestionText = "What changed?",
            Differences = new List<DifferenceDto>
            {
                new() { X = 10, Y = 20 },
                new() { X = 30, Y = 40 }
            },
            Answers = new List<AnswerDto>
            {
                new() { Text = "A", IsCorrect = true },
                new() { Text = "B", IsCorrect = false }
            }
        };

        var round = await service.CreateRoundAsync(request);

        Assert.NotNull(round);
        Assert.Equal("New round", round.Title);
        Assert.Equal(2, round.Differences.Count);
        Assert.Single(round.Questions);
        Assert.Equal(2, round.Questions.First().Answeroptions.Count);
        Assert.Equal("/images/old.png", round.OriginalImage.Path);
        Assert.Equal("/images/new.png", round.DifferenceImage.Path);
    }
}
