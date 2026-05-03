using System;
using System.Collections.Generic;

namespace Spot_The_Difference.Persistence.Entities.MijnMap;

public partial class Difference
{
    public int DifferenceId { get; set; }

    public int RoundId { get; set; }

    public int X { get; set; }

    public int Y { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public virtual Round Round { get; set; } = null!;
}
