using System;
using System.Collections.Generic;

namespace Spot_The_Difference.Persistence.Entities.MijnMap;

public partial class Answeroption
{
    public int AnswerId { get; set; }

    public int QuestionId { get; set; }

    public string Text { get; set; } = null!;
    public string? TextNl { get; set; } // nederlandse vraag
    public string? TextFr { get; set; } // franse vraag

    public bool IsCorrect { get; set; }

    public virtual Question Question { get; set; } = null!;
}
