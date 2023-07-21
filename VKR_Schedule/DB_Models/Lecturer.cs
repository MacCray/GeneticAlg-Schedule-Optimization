using System;
using System.Collections.Generic;

namespace VKR_Schedule.DB_Models;

public partial class Lecturer
{
    public int LecturerId { get; set; }

    public string? LecturerName { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
