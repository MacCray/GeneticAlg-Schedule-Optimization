using System;
using System.Collections.Generic;

namespace VKR_Schedule.DB_Models;

public partial class Time
{
    public int TimeId { get; set; }

    public string TimePeriod { get; set; } = null!;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
