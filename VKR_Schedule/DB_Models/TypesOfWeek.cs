using System;
using System.Collections.Generic;

namespace VKR_Schedule.DB_Models;

public partial class TypesOfWeek
{
    public int TypeOfWeekId { get; set; }

    public string? TypeName { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
