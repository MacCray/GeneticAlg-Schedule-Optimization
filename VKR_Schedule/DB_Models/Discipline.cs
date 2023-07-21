using System;
using System.Collections.Generic;

namespace VKR_Schedule.DB_Models;

public partial class Discipline
{
    public int DisciplineId { get; set; }

    public string DisciplineName { get; set; } = null!;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
