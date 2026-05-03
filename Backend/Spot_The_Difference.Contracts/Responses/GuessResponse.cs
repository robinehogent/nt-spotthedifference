using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot_The_Difference.Contracts.Responses
{
    public class GuessResponse
    {
        public bool IsCorrect { get; set; }
        public bool IsAlreadyFound { get; set; }
        public int DifferencesLeft { get; set; }
        public bool IsGameOver { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
