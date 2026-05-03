using Microsoft.EntityFrameworkCore;
using Spot_The_Difference.Contracts.Requests;
using Spot_The_Difference.Persistence;
using Spot_The_Difference.Persistence.Entities.MijnMap;
using System.Threading.Tasks;
using DB = Spot_The_Difference.Persistence.Entities.MijnMap;

namespace Spot_The_Difference.Domain.Services
{
    public class AdminService
    {
        private readonly SpotthedifferencedbContext _context;

        public AdminService(SpotthedifferencedbContext context)
        {
            _context = context;
        }

        public async Task<DB.Round> CreateRoundAsync(CreateRoundRequest request)
        {
            var difficultyEntity = await _context.Difficulties
                .FirstOrDefaultAsync(d => d.Name == request.Difficulty);

            if (difficultyEntity == null)
            {
                difficultyEntity = await _context.Difficulties.FirstOrDefaultAsync();
            }

            var smallpath = new System.Uri(request.OriginalImage).AbsolutePath; // als ik zou anders willen naam van de path bijhouden, zou ik hier nog iet bij moeten toevoegen nu tis gwn bv /images/myimage.jpg
            var img1 = new DB.Image
            {
                Path = smallpath,
                Type = "original",
                Description = (difficultyEntity?.Name ?? "Admin") + " Before"
            };

            _context.Images.Add(img1);
            await _context.SaveChangesAsync();


            var smallpath2 = new System.Uri(request.DifferenceImage).AbsolutePath;    // same here
            var img2 = new DB.Image
            {
                Path = smallpath2,
                Type = "difference",
                Description = (difficultyEntity?.Name ?? "Admin") + " After"
            };

            _context.Images.Add(img2);
            await _context.SaveChangesAsync();


            var newRound = new DB.Round
            {
                Title = request.Name,
                OriginalImage = img1,
                DifferenceImage = img2,
                Difficulty = difficultyEntity,
                Description = request.QuestionText,
            };

            if (request.Differences != null)
            {
                foreach (var diff in request.Differences)
                {
                    newRound.Differences.Add(new DB.Difference
                    {
                        X = diff.X,
                        Y = diff.Y,
                        Width = 100,
                        Height = 100
                    });
                }
            }

            if (!string.IsNullOrEmpty(request.QuestionText))
            {
                var newQuestion = new DB.Question
                {
                    Text = request.QuestionText
                };

                if (request.Answers != null)
                {
                    foreach (var ans in request.Answers)
                    {
                        newQuestion.Answeroptions.Add(new DB.Answeroption
                        {
                            Text = ans.Text,
                            IsCorrect = ans.IsCorrect
                        });
                    }
                }
                newRound.Questions.Add(newQuestion);
            }
            _context.Rounds.Add(newRound);
            await _context.SaveChangesAsync();

            return newRound;
        }
    }
}