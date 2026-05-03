    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot_The_Difference.Contracts.Responses
{
    public class StartGameResponse
    {
        public int PlayerRoundId { get; set; } 
        public int RoundId { get; set; }
        public string OriginalImageUrl { get; set; } = string.Empty;
        public string DifferenceImageUrl { get; set; } = string.Empty;
        public int TimeLimitSeconds { get; set; }

        public string QuestionText { get; set; } = string.Empty;
        public List<AnswerResponse> Answers { get; set; } = new List<AnswerResponse>();
        public int CorrectAnswerId { get; set; }
    }
}
