using System;
using System.Collections.Generic;

namespace VKR_Schedule.DB_Models;

public partial class Faculty
{
    public int FacultyId { get; set; }

    public string FacultyName { get; set; } = null!;

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
