using System;
using VKR_Schedule.Models;
using VKR_Schedule.Misc;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;

namespace VKR_Schedule.GeneticAlgorithm
{
    public class StudentGroup
    {
        public string GroupId { get; set; }
        public Dictionary<DayOfWeek, List<ScheduleDayInfo>> Schedule { get; set; }
        private static int[] timeSlots = new int[] { 510, 610, 710, 840, 940, 1040, 1140, 1240 };
        public Dictionary<(string course, string instructor), CourseInfo> Curriculum = new();

        public StudentGroup(string groupId)
        {
            GroupId = groupId;
            Schedule = new Dictionary<DayOfWeek, List<ScheduleDayInfo>>();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList().Skip(1))
            {
                Schedule[day] = new List<ScheduleDayInfo>();
            }
        }
        public StudentGroup(StudentGroup group) : this(group.GroupId)
        {
            Curriculum = group.Curriculum;
        }

        public void FillSchedule()
        {
            var rand = new Random();

            foreach (var course in Curriculum)
            {
                var courseCount1 = Schedule.Values.SelectMany(s => s).Where(s => s.Course.Name == course.Key.course && s.Instructor == course.Key.instructor && s.WeekType == null).Count();
                var courseCount2 = Schedule.Values.SelectMany(s => s).Where(s => s.Course.Name == course.Key.course && s.Instructor == course.Key.instructor && s.WeekType == "Верхняя неделя").Count();
                var courseCount3 = Schedule.Values.SelectMany(s => s).Where(s => s.Course.Name == course.Key.course && s.Instructor == course.Key.instructor && s.WeekType == "Нижняя неделя").Count();

                while (courseCount1 != course.Value.Count2["Обе недели"])
                {
                    var day = (DayOfWeek)rand.Next(1, 7);
                    AddCourse(day, "Обе недели", course, ref courseCount1, Shuffle(timeSlots), true);
                }
                while (courseCount2 != course.Value.Count2["Верхняя неделя"])
                {
                    var day = (DayOfWeek)rand.Next(1, 7);
                    AddCourse(day, "Верхняя неделя", course, ref courseCount2, Shuffle(timeSlots), true);
                }
                while (courseCount3 != course.Value.Count2["Нижняя неделя"])
                {
                    var day = (DayOfWeek)rand.Next(1, 7);
                    AddCourse(day, "Нижняя неделя", course, ref courseCount3, Shuffle(timeSlots), true);
                }
            }
        }

        public void CheckSchedule()
        {
            foreach (var course in Curriculum)
            {
                var courseCount1 = Schedule.Values.SelectMany(s => s)
                    .Count(s => s.Course.Name == course.Key.course && s.Instructor == course.Key.instructor && s.WeekType == null);
                var courseCount2 = Schedule.Values.SelectMany(s => s)
                    .Count(s => s.Course.Name == course.Key.course && s.Instructor == course.Key.instructor && s.WeekType == "Верхняя неделя");
                var courseCount3 = Schedule.Values.SelectMany(s => s)
                    .Count(s => s.Course.Name == course.Key.course && s.Instructor == course.Key.instructor && s.WeekType == "Нижняя неделя");

                if (courseCount1 != course.Value.Count2["Обе недели"] || courseCount2 != course.Value.Count2["Верхняя неделя"] || courseCount3 != course.Value.Count2["Нижняя неделя"])
                    ScheduleRepair();
            }
        }

        private void ScheduleRepair()
        {
            foreach (var course in Curriculum)
            {
                var courseCount1 = Schedule.Values.SelectMany(s => s).Count(s => s.Course.Name == course.Key.course && s.Instructor == course.Key.instructor && s.WeekType == null);
                var courseCount2 = Schedule.Values.SelectMany(s => s).Count(s => s.Course.Name == course.Key.course && s.Instructor == course.Key.instructor && s.WeekType == "Верхняя неделя");
                var courseCount3 = Schedule.Values.SelectMany(s => s).Count(s => s.Course.Name == course.Key.course && s.Instructor == course.Key.instructor && s.WeekType == "Нижняя неделя");

                if (courseCount1 > course.Value.Count2["Обе недели"])
                {
                    RemoveCourse(course.Key, courseCount1 - course.Value.Count2["Обе недели"], null);
                }
                if (courseCount2 > course.Value.Count2["Верхняя неделя"])
                {
                    RemoveCourse(course.Key, courseCount2 - course.Value.Count2["Верхняя неделя"], "Верхняя неделя");
                }
                if (courseCount3 > course.Value.Count2["Нижняя неделя"])
                {
                    RemoveCourse(course.Key, courseCount3 - course.Value.Count2["Нижняя неделя"], "Нижняя неделя");
                }
            }

            foreach (var course in Curriculum)
            {
                var courseCount1 = Schedule.Values.SelectMany(s => s).Count(s => s.Course.Name == course.Key.course && s.Instructor == course.Key.instructor && s.WeekType == null);
                var courseCount2 = Schedule.Values.SelectMany(s => s).Count(s => s.Course.Name == course.Key.course && s.Instructor == course.Key.instructor && s.WeekType == "Верхняя неделя");
                var courseCount3 = Schedule.Values.SelectMany(s => s).Count(s => s.Course.Name == course.Key.course && s.Instructor == course.Key.instructor && s.WeekType == "Нижняя неделя");

                while (courseCount1 < course.Value.Count2["Обе недели"])
                {
                    foreach (var day in Schedule)
                    {
                        AddCourse(day.Key, "Обе недели", course, ref courseCount1, timeSlots);
                        if (courseCount1 == course.Value.Count2["Обе недели"]) break;
                    }
                }
                while (courseCount2 < course.Value.Count2["Верхняя неделя"])
                {
                    foreach (var day in Schedule)
                    {
                        AddCourse(day.Key, "Верхняя неделя", course, ref courseCount2, timeSlots);
                        if (courseCount2 == course.Value.Count2["Верхняя неделя"]) break;
                    }
                }
                while (courseCount3 < course.Value.Count2["Нижняя неделя"])
                {
                    foreach (var day in Schedule)
                    {
                        AddCourse(day.Key, "Нижняя неделя", course, ref courseCount3, timeSlots);
                        if (courseCount3 == course.Value.Count2["Нижняя неделя"]) break;
                    }
                }
            }
        }

        private void AddCourse(DayOfWeek day, string WeekType, in KeyValuePair<(string course, string instructor), CourseInfo> course, ref int courseCount, IEnumerable<int> timeSlots, bool Filling = false)
        {
            IEnumerable<ScheduleDayInfo> daySchedule;
            string? weekT = WeekType;
            bool available = false;
            if (WeekType == "Обе недели")
            {
                weekT = null;
                daySchedule = Schedule[day];
                if (daySchedule.Count(s => s.WeekType is null or "Верхняя неделя") < 6 || daySchedule.Count(s => s.WeekType is null or "Нижняя неделя") < 6)
                {
                    available = true;
                }
            }
            else
                daySchedule = Schedule[day].Where(s => s.WeekType == WeekType || s.WeekType == null);

            if (daySchedule.Count() < 6 || available)
            {
                IEnumerable<int> freeTimeSlots;
                if (!daySchedule.Any())
                    freeTimeSlots = timeSlots.Take(Math.Min(course.Value.Count2[WeekType] - courseCount, 6 - daySchedule.Count()));
                else
                {
                    var occupiedTimeSlots = daySchedule.Select(s => s.TimeSlot.StartHour * 60 + s.TimeSlot.StartMinute);
                    if (Filling)
                    {
                        freeTimeSlots = timeSlots.Except(occupiedTimeSlots)
                            .Take(Math.Min(course.Value.Count2[WeekType] - courseCount, 6 - daySchedule.Count()));
                    }
                    else
                    {
                        if (available)
                        {
                            freeTimeSlots = timeSlots.Except(occupiedTimeSlots)
                                        .OrderBy(x => occupiedTimeSlots.Select(y => Math.Abs(y - x)).Min())
                                        .Take(Math.Min(course.Value.Count2[WeekType] - courseCount, 6 - Math.Min(
                                            daySchedule.Count(s => s.WeekType is null or "Верхняя неделя"),
                                            daySchedule.Count(s => s.WeekType is null or "Нижняя неделя"))));
                        }
                        else
                        {
                            freeTimeSlots = timeSlots.Except(occupiedTimeSlots)
                                        .OrderBy(x => occupiedTimeSlots.Select(y => Math.Abs(y - x)).Min())
                                        .Take(Math.Min(course.Value.Count2[WeekType] - courseCount, 6 - daySchedule.Count()));
                        }
                    }
                }

                foreach (var timeSlot in freeTimeSlots)
                {
                    Random rnd = new();
                    var room = course.Value.Rooms.ElementAt(rnd.Next(course.Value.Rooms.Count));
                    Schedule[day].Add(new ScheduleDayInfo()
                    {
                        Day = day,
                        GroupId = GroupId,
                        TimeSlot = new TimeSlot(timeSlot / 60, timeSlot % 60),
                        WeekType = weekT,
                        Room = room,
                        Course = new(course.Key.course),
                        Instructor = course.Key.instructor,
                    });
                    courseCount++;
                }
            }
        }

        private void RemoveCourse((string course, string instructor) key, int count, string? weekType)
        {
            Random rnd = new();
            var duplicateLessons = Schedule.Values.SelectMany(s => s).Where(s => s.Course.Name == key.course && s.Instructor == key.instructor && s.WeekType == weekType).ToList();

            for (int i = 0; i < count; i++)
            {
                var lessonIndex = rnd.Next(duplicateLessons.Count);
                Schedule[duplicateLessons[lessonIndex].Day].Remove(duplicateLessons[lessonIndex]);
            }
        }

        private static IEnumerable<int> Shuffle(in int[] listToShuffle)
        {
            var result = new int[listToShuffle.Length];
            listToShuffle.CopyTo(result, 0);
            Random rnd = new();
            for (int i = result.Length - 1; i > 0; i--)
            {
                var k = rnd.Next(i + 1);
                (result[i], result[k]) = (result[k], result[i]);
            }
            return result;
        }

        public override string ToString()
        {
            return "Группа " + GroupId;
        }
        public string PrintSchedule()
        {
            StringBuilder result = new();
            foreach (var day in Schedule)
            {
                result.AppendLine(DateTimeFormatInfo.CurrentInfo.GetDayName(day.Key));
                foreach (var lesson in day.Value)
                {
                    result.AppendLine(lesson.Print());
                }
                result.AppendLine("\n");
            }
            return result.ToString();
        }
    }
}