using System;
using System.Collections.Generic;

namespace VKR_Schedule.DB_Models;

public partial class Auditoria
{
    public int AuditoriaId { get; set; }

    public string AuditoriaName { get; set; } = null!;

    public string? AuditoriaBuilding { get; set; }

    public bool? IsOnMap { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
