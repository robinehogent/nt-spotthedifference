using System;
using System.Collections.Generic;

namespace Spot_The_Difference.Persistence.Entities.MijnMap;

public partial class Image
{
    public int ImageId { get; set; }

    public string Path { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Round> RoundDifferenceImages { get; set; } = new List<Round>();

    public virtual ICollection<Round> RoundOriginalImages { get; set; } = new List<Round>();
}
