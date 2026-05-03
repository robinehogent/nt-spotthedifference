using System;
using System.Collections.Generic;

namespace Spot_The_Difference.Persistence.Entities.MijnMap;

public partial class Playerround
{
    public int PlayerRoundId { get; set; }

    public int PlayerId { get; set; }

    public int RoundId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? TimeSpent { get; set; }

    public int? Score { get; set; }

    public bool? Completed { get; set; }

    public int? CorrectQuestions { get; set; }

    public int? CorrectDifferences { get; set; }

    public virtual Player Player { get; set; } = null!;

    public virtual Round Round { get; set; } = null!;
}
