using System;
using System.Collections.Generic;

namespace VKR_Schedule.DB_Models;

public partial class DaysOfWeek
{
    public int DayOfWeekId { get; set; }

    public string? DayOfWeekName { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
