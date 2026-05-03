using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot_The_Difference.Contracts.Requests
{
    public class DifferenceDto // gka later cleanen aka Artem
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class AnswerDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class CreateRoundRequest
    {
        public string Name { get; set; } = string.Empty;
        public string OriginalImage { get; set; } = string.Empty;
        public string DifferenceImage { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public List<DifferenceDto> Differences { get; set; } = new List<DifferenceDto>();
        public string QuestionText { get; set; } = string.Empty;
        public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
    }
}
