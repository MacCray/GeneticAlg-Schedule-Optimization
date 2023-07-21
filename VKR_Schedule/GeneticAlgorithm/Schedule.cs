using VKR_Schedule.Misc;

namespace VKR_Schedule.GeneticAlgorithm
{
    public class Schedule
    {
        public List<StudentGroup> StudentGroups { get; set; }
        public float Fitness { get; private set; }
        public float TotalPenalties { get; private set; }
        public float Conflicts { get; private set; }
        public float Breaks { get; private set; }
        public float Transfer { get; private set; }
        public float Infections { get; private set; }

        public Schedule()
        {
            StudentGroups = new List<StudentGroup>();
        }

        public Schedule(List<StudentGroup> studentGroups)
        {
            StudentGroups = studentGroups;
        }

        public void CalculateFitness(bool calcInf, float conflictWeight = 1.0f, float breaksPenaltyWeight = 1.0f, float transferTimePenaltyWeight = 1.0f, float infectionsWeight = 1.0f, bool originalSchedule = false)
        {
            if (originalSchedule)
            {
                CalculateConflicts(originalSchedule);
                return;
            }

            int conflicts = CalculateConflicts();
            Conflicts = conflicts * conflictWeight;

            (int breaksPenalties, int transferTimePenalties) = CalculateBreaksPenalty();
            Breaks = breaksPenalties * breaksPenaltyWeight;
            Transfer = transferTimePenalties * transferTimePenaltyWeight;

            int infections = calcInf ? CalculateInfections() : 0;
            Infections = infections * infectionsWeight;

            TotalPenalties = conflicts * conflictWeight + breaksPenalties * breaksPenaltyWeight + transferTimePenalties * transferTimePenaltyWeight + infections * infectionsWeight;
            Fitness = 1 / TotalPenalties;
        }

        private int CalculateConflicts(bool originalSchedule = false)
        {
            int conflicts = 0;
            int r = 0; int l = 0;
            for (int i = 0; i < StudentGroups.Count - 1; i++)
            {
                StudentGroup currentGroup = StudentGroups[i];

                foreach (var day in currentGroup.Schedule.Keys.Where(day => currentGroup.Schedule[day].Count != 0))
                {
                    var groupsWithDay = StudentGroups.Skip(i + 1).Where(group => group.Schedule[day].Count != 0);

                    foreach (var otherGroup in groupsWithDay)
                    {
                        Dictionary<ScheduleDayInfo, List<ScheduleDayInfo>> scheduleIntersects = new();
                        foreach (var otherGroupInfo in otherGroup.Schedule[day])
                        {
                            var intersects = currentGroup.Schedule[day].Where(s => s.TimeSlot.StartHour == otherGroupInfo.TimeSlot.StartHour && (s.WeekType == otherGroupInfo.WeekType || s.WeekType == null));
                            if (intersects.Any())
                            {
                                scheduleIntersects.Add(otherGroupInfo, intersects.ToList());
                            }
                        }

                        if (scheduleIntersects.Any())
                        {
                            if (originalSchedule)
                            {
                                foreach (var intersect in scheduleIntersects)
                                {
                                    intersect.Value.ForEach(s =>
                                    {
                                        if (s.Course.Name == intersect.Key.Course.Name
                                        && s.Room.RoomNumber == intersect.Key.Room.RoomNumber
                                        && s.Instructor == intersect.Key.Instructor)
                                        {
                                            intersect.Key.MultipleGroups = true;
                                            s.MultipleGroups = true;
                                        }
                                    });
                                }
                            }
                            else
                            {
                                List<ScheduleDayInfo> lessonsIntersects = new();
                                foreach (var intersect in scheduleIntersects)
                                {
                                    intersect.Value.ForEach(s =>
                                    {
                                        if ((s.Room.RoomNumber == intersect.Key.Room.RoomNumber || s.Instructor == intersect.Key.Instructor) &&
                                            !((s.Room.RoomNumber == intersect.Key.Room.RoomNumber) && (s.Instructor == intersect.Key.Instructor) &&
                                            (s.Course.Name == intersect.Key.Course.Name) && intersect.Key.MultipleGroups && s.MultipleGroups) &&
                                            !(s.Room.RoomNumber == "ЦДО Дистанционно" && intersect.Key.Room.RoomNumber == "ЦДО Дистанционно" &&
                                            (s.Instructor != intersect.Key.Instructor)))
                                        {
                                            if (!(s.Instructor == intersect.Key.Instructor && intersect.Key.Instructor == "")
                                            && !(s.Room.RoomNumber == intersect.Key.Room.RoomNumber && (s.Room.RoomNumber.Contains("каф.ИЯ")
                                                                                                        || s.Room.RoomNumber.Contains("каф.ФВ")
                                                                                                        || s.Room.RoomNumber == "ЦДО Дистанционно")))
                                            {
                                                if (!lessonsIntersects.Contains(intersect.Key))
                                                    lessonsIntersects.Add(intersect.Key);
                                                lessonsIntersects.Add(s);
                                            }
                                        }
                                    });
                                }
                                var RoomConflicts = lessonsIntersects.GroupBy(x => x.Room.RoomNumber);
                                var InstructorConflicts = lessonsIntersects.GroupBy(x => x.Instructor);
                                foreach (var rC in RoomConflicts)
                                {
                                    if (rC.Count() > 1)
                                    {
                                        conflicts += rC.Count();
                                        r += rC.Count();
                                    }

                                }
                                foreach (var iC in InstructorConflicts)
                                {
                                    if (iC.Count() > 1)
                                    {
                                        conflicts += iC.Count();
                                        l += iC.Count();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return conflicts;
        }

        private (int totalBreaksPenalties, int totalTransferTimePenalties) CalculateBreaksPenalty()
        {
            int breaksPenalty = 0;
            int transferTimePenalty = 0;

            foreach (var group in StudentGroups)
            {
                foreach (var day in group.Schedule.Keys.Where(day => group.Schedule[day].Count != 0))
                {
                    Dictionary<int, List<ScheduleDayInfo>> lessons = new()
                    {
                        { 1, group.Schedule[day].Where(x => x.WeekType is "Верхняя неделя" or null)
                                                .OrderBy(scheduleInfo => scheduleInfo.TimeSlot.StartHour).ToList() },
                        { 2, group.Schedule[day].Where(x => x.WeekType is "Нижняя неделя" or null)
                                                .OrderBy(scheduleInfo => scheduleInfo.TimeSlot.StartHour).ToList() }
                    };
                    foreach (var lessonsForTheDay in lessons)
                    {
                        for (int i = 1; i < lessonsForTheDay.Value.Count; i++)
                        {
                            int lessonEndTimePrev = lessonsForTheDay.Value[i - 1].TimeSlot.EndHour * 60 + lessonsForTheDay.Value[i - 1].TimeSlot.EndMinute;
                            int lessonStartTime = lessonsForTheDay.Value[i].TimeSlot.StartHour * 60 + lessonsForTheDay.Value[i].TimeSlot.StartMinute;

                            if (lessonsForTheDay.Value[i - 1].Room.Building == lessonsForTheDay.Value[i].Room.Building)
                            {
                                if (lessonStartTime - lessonEndTimePrev > 40)
                                {
                                    if (lessonStartTime == 840 || lessonEndTimePrev == 800 || (lessonStartTime > 840 && lessonEndTimePrev < 800))
                                        breaksPenalty += lessonStartTime - lessonEndTimePrev - 40;
                                    else
                                        breaksPenalty += lessonStartTime - lessonEndTimePrev;
                                }
                                else if (lessonStartTime - lessonEndTimePrev < 0)
                                { }
                            }
                            else
                            {
                                if (lessonStartTime - lessonEndTimePrev > 140)
                                    transferTimePenalty += lessonStartTime - lessonEndTimePrev - 110;
                                else if (lessonStartTime - lessonEndTimePrev < 110)
                                    transferTimePenalty += 110;
                            }
                        }
                    }
                }
            }
            return (breaksPenalty, transferTimePenalty);
        }

        public int CalculateInfections()
        {
            var DateStartAnalize = new DateTime(2021, 10, 4);
            var DateStopAnalize = new DateTime(2021, 10, 9);
            AnalyzeInfections analyze;
            List<int> maxInfByGroup = new();
            int i = 0;
            var groups = StudentGroups.Where(g => g.GroupId.StartsWith("20"));

            foreach (var g in groups)
            {
                maxInfByGroup.Add(0);
                for (DateTime date = DateStartAnalize; date <= DateStopAnalize; date = date.AddDays(1))
                {
                    analyze = new(date, date.AddDays(3), date.AddDays(2), g, StudentGroups);
                    int result = analyze.MakeResearch();
                    if (result > maxInfByGroup[i]) maxInfByGroup[i] = result;
                }
                i++;
            }
            Infections = maxInfByGroup.Sum() / (i + 1);
            return maxInfByGroup.Sum() / (i + 1);
        }
    }
}
