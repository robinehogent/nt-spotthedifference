using Microsoft.EntityFrameworkCore;
using Spot_The_Difference.Contracts.Requests;
using Spot_The_Difference.Contracts.Responses;
using Spot_The_Difference.Persistence;
using Spot_The_Difference.Persistence.Entities.MijnMap;
using System;
using System.Linq;
using System.Threading.Tasks;
using DB = Spot_The_Difference.Persistence.Entities.MijnMap;

namespace Spot_The_Difference.Domain.Services
{
    public class GameService
    {
        private readonly SpotthedifferencedbContext _context;
        private readonly LevelService _levelService;

        public GameService(SpotthedifferencedbContext context, LevelService levelService)
        {
            _context = context;
            _levelService = levelService;
        }

        public async Task<StartGameResponse?> StartNewGameAsync(StartGameRequest request)
        {
            var player = await _context.Players
                .FirstOrDefaultAsync(p => p.Username == request.Username);

            if (player == null)
            {
                player = new DB.Player { Username = request.Username };
                _context.Players.Add(player);
                await _context.SaveChangesAsync();
            }

            var playedRoundIds = await _context.Playerrounds
                .Where(pr => pr.PlayerId == player.PlayerId && pr.Completed == true)
                .Select(pr => pr.RoundId)
                .ToListAsync();

            var selectedRound = await _context.Rounds
                .Include(r => r.Difficulty)
                .Include(r => r.OriginalImage)
                .Include(r => r.DifferenceImage)
                .Include(r => r.Questions).ThenInclude(q => q.Answeroptions)
                .Where(r => r.DifficultyId == request.DifficultyId)
                .Where(r => !playedRoundIds.Contains(r.RoundId))
                .FirstOrDefaultAsync();

            if (selectedRound == null)
            {
                bool anyLevelExists = await _context.Rounds
                    .AnyAsync(r => r.DifficultyId == request.DifficultyId);

                //if (anyLevelExists)
                //{
                //    throw new Exception("Je hebt alle niveaus van deze moeilijkheidsgraad al voltooid!");
                //}

                //return null;

                if (selectedRound == null)
                {
                    return null;
                }
            }

            var session = new DB.Playerround
            {
                PlayerId = player.PlayerId,
                RoundId = selectedRound.RoundId,
                StartTime = DateTime.Now,
                Score = 0,
                Completed = false
            };

            _context.Playerrounds.Add(session);
            await _context.SaveChangesAsync();

            var question = selectedRound.Questions.FirstOrDefault();

            // Helper methods for language selection
            string GetQuestionText(Question q, string lang)
            {
                return lang switch
                {
                    "nl" => q.TextNl ?? q.Text,
                    "fr" => q.TextFr ?? q.Text,
                    _ => q.Text
                };
            }
            string GetAnswerText(Answeroption a, string lang)
            {
                return lang switch
                {
                    "nl" => a.TextNl ?? a.Text,
                    "fr" => a.TextFr ?? a.Text,
                    _ => a.Text
                };
            }

            string language = request.Language?.ToLower() ?? "en";

            return new StartGameResponse
            {
                PlayerRoundId = session.PlayerRoundId,
                RoundId = selectedRound.RoundId,

                OriginalImageUrl = _levelService.GetFullUrl(selectedRound.OriginalImage.Path),
                DifferenceImageUrl = _levelService.GetFullUrl(selectedRound.DifferenceImage.Path),

                TimeLimitSeconds = selectedRound.Difficulty.TimeLimitSeconds,

                QuestionText = question != null ? GetQuestionText(question, language) : "",

                Answers = question?.Answeroptions
        .Select(a => new AnswerResponse
        {
            AnswerId = a.AnswerId,
            Text = GetAnswerText(a, language)
        })
        .ToList() ?? new List<AnswerResponse>(),

                CorrectAnswerId = question?.Answeroptions
        .FirstOrDefault(a => a.IsCorrect)?.AnswerId ?? 0
            };
        }


        public async Task<List<LeaderboardEntry>> GetLeaderboardAsync()
        {
            return await _context.Playerrounds
                .Include(pr => pr.Player)
                .Include(pr => pr.Round).ThenInclude(r => r.Difficulty)
                .Where(pr => pr.Completed == true)
                .OrderByDescending(pr => pr.Score) 
                .ThenBy(pr => pr.TimeSpent)        
                .Take(20)                          
                .Select(pr => new LeaderboardEntry
                {
                    Username = pr.Player.Username,
                    Difficulty = pr.Round.Difficulty.Name,
                    Score = pr.Score,
                    TimeSpentSeconds = pr.TimeSpent,
                    DatePlayed = pr.StartTime
                })
                .ToListAsync();
        }

        public async Task<GuessResponse?> ProcessGuessAsync(GuessRequest request)
        {
            var playerRound = await _context.Playerrounds
                .FirstOrDefaultAsync(pr => pr.PlayerRoundId == request.PlayerRoundId);

            if (playerRound == null) return null;

            var differences = await _context.Differences
                .Where(d => d.RoundId == playerRound.RoundId)
                .ToListAsync();

            foreach (var diff in differences)
            {
                int halfWidth = diff.Width / 2;
                int halfHeight = diff.Height / 2;

                bool isHitX = request.X >= (diff.X - halfWidth) && request.X <= (diff.X + halfWidth);
                bool isHitY = request.Y >= (diff.Y - halfHeight) && request.Y <= (diff.Y + halfHeight);

                if (isHitX && isHitY)
                {
                    if (playerRound.Completed == true)
                    {
                        return new GuessResponse
                        {
                            IsCorrect = true,
                            IsAlreadyFound = true,
                            Message = "Je hebt dit al gevonden!",
                            DifferencesLeft = 0,
                            IsGameOver = true
                        };
                    }

                    playerRound.EndTime = DateTime.Now;
                    if (playerRound.StartTime != DateTime.MinValue)
                    {
                        var duration = playerRound.EndTime - playerRound.StartTime;
                        if (duration.HasValue) playerRound.TimeSpent = (int)duration.Value.TotalSeconds;
                    }

                    playerRound.Completed = true;
                    playerRound.CorrectDifferences += 1;
                    playerRound.Score += 100;

                    await _context.SaveChangesAsync();
                    return new GuessResponse
                    {
                        IsCorrect = true,
                        Message = $"Gefeliciteerd! Tijd: {playerRound.TimeSpent}s",
                        DifferencesLeft = 0,
                        IsGameOver = true
                    };
                }
            }

            playerRound.Score -= 10;
            await _context.SaveChangesAsync();

            return new GuessResponse
            {
                IsCorrect = false,
                Message = "Mis!",
                DifferencesLeft = differences.Count
            };
        }
    }

    public class LeaderboardEntry
    {
        public string Username { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int ?Score { get; set; }
        public int ?TimeSpentSeconds { get; set; }
        public DateTime ?DatePlayed { get; set; }
    }
}