using System;
using System.Collections.Generic;

namespace Spot_The_Difference.Persistence.Entities.MijnMap;

public partial class Differenceoption
{
    public int OptionId { get; set; }

    public int RoundId { get; set; }

    public string Text { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public virtual Round Round { get; set; } = null!;
}
