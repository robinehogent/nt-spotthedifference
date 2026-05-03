using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot_The_Difference.Contracts.Requests
{
    public class GuessRequest
    {
        public int PlayerRoundId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
