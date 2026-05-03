using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot_The_Difference.Contracts.Requests
{
    public class StartGameRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Language { get; set; } = "en"; // Default to English

        public int DifficultyId { get; set; }
    }
}
