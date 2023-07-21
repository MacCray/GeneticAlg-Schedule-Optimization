using System;
using System.Collections.Generic;

namespace VKR_Schedule.DB_Models;

public partial class Group
{
    public int GroupId { get; set; }

    public string GroupNumber { get; set; } = null!;

    public int FacultyId { get; set; }

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
