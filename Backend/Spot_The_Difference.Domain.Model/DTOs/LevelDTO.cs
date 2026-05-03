using Spot_The_Difference.Contracts.Responses;

namespace Spot_The_Difference.Domain.Model.DTOs
{
    public class LevelDTO
    {
        public int Id { get; set; }
        public string Image1 { get; set; } = string.Empty;
        public string Image2 { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public List<AnswerResponse> Answers { get; set; } = new();
        public int CorrectAnswerId { get; set; }

        public int TimeLimitSeconds { get; set; }   

    }

}
