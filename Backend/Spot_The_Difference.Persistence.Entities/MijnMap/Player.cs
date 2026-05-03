using System;
using System.Collections.Generic;

namespace Spot_The_Difference.Persistence.Entities.MijnMap;

public partial class Player
{
    public int PlayerId { get; set; }

    public string Username { get; set; } = null!;

    public virtual ICollection<Playerround> Playerrounds { get; set; } = new List<Playerround>();
}
