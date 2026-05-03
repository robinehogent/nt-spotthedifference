using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Spot_The_Difference.Contracts.Responses;
using Spot_The_Difference.Domain.Model.DTOs;
using Spot_The_Difference.Persistence.Entities.MijnMap;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spot_The_Difference.Domain.Services
{
    public class LevelService
    {
        private readonly SpotthedifferencedbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LevelService(SpotthedifferencedbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetFullUrl(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return string.Empty;
            var request = _httpContextAccessor.HttpContext!.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            if (!relativePath.StartsWith("/")) relativePath = "/" + relativePath;
            return $"{baseUrl}{relativePath}";
        }

        public async Task<LevelDTO> GetLevel(int id, string language = "en")
        {
            var round = await _context.Rounds
                .Include(r => r.OriginalImage)
                .Include(r => r.DifferenceImage)
                .Include(r => r.Questions).ThenInclude(q => q.Answeroptions)
                .Include(r => r.Difficulty)
                .FirstOrDefaultAsync(r => r.RoundId == id);

            if (round == null) return null;

            var question = round.Questions.FirstOrDefault();

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
            var correctAnswer = question?.Answeroptions.FirstOrDefault(a => a.IsCorrect);

            return new LevelDTO
            {
                Id = round.RoundId,
                Image1 = GetFullUrl(round.OriginalImage.Path),
                Image2 = GetFullUrl(round.DifferenceImage.Path),
                Question = question != null ? GetQuestionText(question, language) : "",

                Answers = question?.Answeroptions.Select(a => new AnswerResponse
                {
                    AnswerId = a.AnswerId,
                    Text = GetAnswerText(a, language)
                }).ToList() ?? new List<AnswerResponse>(),

                CorrectAnswerId = correctAnswer?.AnswerId ?? 0,

                TimeLimitSeconds = round.Difficulty.TimeLimitSeconds
            };
        }
    }
}