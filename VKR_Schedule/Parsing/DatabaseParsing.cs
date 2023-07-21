using Microsoft.EntityFrameworkCore;
using VKR_Schedule.DB_Models;
using VKR_Schedule.GeneticAlgorithm;
using VKR_Schedule.Models;
using VKR_Schedule.Misc;

namespace VKR_Schedule.Parsing
{
    internal static class DatabaseParsing
    {
        public static GeneticAlgorithm.Schedule ParseSchedule()
        {
            GeneticAlgorithm.Schedule schedule = new();
            using (var dbContext = new ScheduleDbContext())
            {
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                var groups = dbContext.Groups
                .Include(g => g.Schedules)
                .ThenInclude(l => l.Lecturer)
                .Include(g => g.Schedules)
                .ThenInclude(d => d.Discipline)
                .Include(g => g.Schedules)
                .ThenInclude(a => a.Auditoria)
                .Include(g => g.Schedules)
                .ThenInclude(t => t.Time)
                .Include(g => g.Schedules)
                .ThenInclude(t => t.TypeOfWeek)
                .Include(g => g.Schedules)
                .ThenInclude(d => d.DayOfWeek);

                foreach (var group in groups)
                {
                    StudentGroup studentGroup = new(group.GroupNumber);
                    foreach (var groupSchedule in group.Schedules)
                    {
                        var scheduleForDay = new ScheduleDayInfo()
                        {
                            Day = (DayOfWeek)groupSchedule.DayOfWeekId,
                            GroupId = group.GroupNumber,
                            TimeSlot = new TimeSlot(groupSchedule.Time.TimePeriod),
                            WeekType = groupSchedule.TypeOfWeek?.TypeName,
                            Room = new Room(groupSchedule.Auditoria.AuditoriaName, groupSchedule.Auditoria.AuditoriaBuilding),
                            Course = new Course(groupSchedule.Discipline.DisciplineName),
                            Instructor = groupSchedule.Lecturer.LecturerName,
                        };
                        studentGroup.Schedule[(DayOfWeek)groupSchedule.DayOfWeekId].Add(scheduleForDay);

                        (string, string) key = (scheduleForDay.Course.Name, scheduleForDay.Instructor);
                        if (studentGroup.Curriculum.ContainsKey(key))
                        {
                            if(scheduleForDay.WeekType == null)
                                studentGroup.Curriculum[key].Count2["Обе недели"]++;
                            else
                                studentGroup.Curriculum[key].Count2[scheduleForDay.WeekType]++;
                            if (!studentGroup.Curriculum[key].Rooms.Any(r => r.RoomNumber == scheduleForDay.Room.RoomNumber))
                                studentGroup.Curriculum[key].Rooms.Add(scheduleForDay.Room);
                        }
                        else
                            studentGroup.Curriculum[key] = new CourseInfo(scheduleForDay.Room, scheduleForDay.WeekType);
                    }
                    schedule.StudentGroups.Add(studentGroup);
                }
            }
            schedule.CalculateFitness(calcInf:false, originalSchedule: true);
            return schedule;
        }
    }
}
