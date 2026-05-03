using System;
using System.Collections.Generic;

namespace Spot_The_Difference.Persistence.Entities.MijnMap;

public partial class Round
{
    public int RoundId { get; set; }

    public int DifficultyId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int OriginalImageId { get; set; }

    public int DifferenceImageId { get; set; }

    public virtual Image DifferenceImage { get; set; } = null!;

    public virtual ICollection<Differenceoption> Differenceoptions { get; set; } = new List<Differenceoption>();

    public virtual ICollection<Difference> Differences { get; set; } = new List<Difference>();

    public virtual Difficulty Difficulty { get; set; } = null!;

    public virtual Image OriginalImage { get; set; } = null!;

    public virtual ICollection<Playerround> Playerrounds { get; set; } = new List<Playerround>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
