using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot_The_Difference.Contracts.Responses
{
    public class AnswerResponse
    {
        public int AnswerId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
