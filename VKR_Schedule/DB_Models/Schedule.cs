using System;
using System.Collections.Generic;

namespace VKR_Schedule.DB_Models;

public partial class Schedule
{
    public int SсheduleId { get; set; }

    public int GroupId { get; set; }

    public int? DayOfWeekId { get; set; }

    public int? TimeId { get; set; }

    public int? TypeOfWeekId { get; set; }

    public int? AuditoriaId { get; set; }

    public int? DisciplineId { get; set; }

    public int? LecturerId { get; set; }

    public virtual Auditoria? Auditoria { get; set; }

    public virtual DaysOfWeek? DayOfWeek { get; set; }

    public virtual Discipline? Discipline { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Lecturer? Lecturer { get; set; }

    public virtual Time? Time { get; set; }

    public virtual TypesOfWeek? TypeOfWeek { get; set; }
}
