using VKR_Schedule.Models;

namespace VKR_Schedule.GeneticAlgorithm
{
    public class ScheduleDayInfo : ICloneable<ScheduleDayInfo>
    {
        public DayOfWeek Day { get; set; }
        public string GroupId { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public string? WeekType { get; set; }
        public Room Room { get; set; }
        public Course Course { get; set; }
        public string Instructor { get; set; }
        public bool MultipleGroups { get; set; }

        public ScheduleDayInfo Clone()
        {
            return new ScheduleDayInfo
            {
                Day = Day,
                GroupId = GroupId,
                TimeSlot = new TimeSlot(TimeSlot.StartHour, TimeSlot.StartMinute),
                WeekType = WeekType,
                Room = Room,
                Course = Course,
                Instructor = Instructor,
                MultipleGroups = MultipleGroups
            };
        }

        public string Print()
        {
            return $"{TimeSlot.ToString()} {WeekType ?? "Обе недели"} {Room.RoomNumber} {Course.Name} {Instructor}";
        }

        public override bool Equals(object? obj) => obj is ScheduleDayInfo other && this.Equals(other);
        public bool Equals(ScheduleDayInfo scheduleDayInfo) => (Day == scheduleDayInfo.Day) && (TimeSlot.StartHour == scheduleDayInfo.TimeSlot.StartHour) && (WeekType == scheduleDayInfo.WeekType) && (Room.RoomNumber == scheduleDayInfo.Room.RoomNumber) && (Course.Name == scheduleDayInfo.Course.Name)
            && (Instructor == scheduleDayInfo.Instructor);
    }

    internal interface ICloneable<T>
    {
        T Clone();
    }
}
