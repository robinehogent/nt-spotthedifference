using System;
using System.Collections.Generic;

namespace Spot_The_Difference.Persistence.Entities.MijnMap;

public partial class Difficulty
{
    public int DifficultyId { get; set; }

    public string Name { get; set; } = null!;

    public int QuestionCount { get; set; }

    public int DifferenceCount { get; set; }

    public int TimeLimitSeconds { get; set; }

    public virtual ICollection<Round> Rounds { get; set; } = new List<Round>();
}
