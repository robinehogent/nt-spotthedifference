using System;
using System.Collections.Generic;

namespace Spot_The_Difference.Persistence.Entities.MijnMap;

public partial class Question
{
    public int QuestionId { get; set; }

    public int RoundId { get; set; }

    public string Text { get; set; } = null!;
    public string? TextNl { get; set; } // nederlandse vraag
    public string? TextFr { get; set; } // franse vraag 

    public virtual ICollection<Answeroption> Answeroptions { get; set; } = new List<Answeroption>();

    public virtual Round Round { get; set; } = null!;
}
