using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VKR_Schedule.GeneticAlgorithm;
using VKR_Schedule.Models;

namespace VKR_Schedule.Misc
{
    public class CourseInfo
    {
        public int Count { get; set; }
        public Dictionary<string, int> Count2 { get; set; }
        public List<Room> Rooms { get; set; } = new List<Room>();

        public CourseInfo(Room room)
        {
            Count = 1;
            Rooms.Add(room);
        }

        public CourseInfo(Room room, string week)
        {
            Count2 = new()
            {
                { "Обе недели", 0 },
                { "Верхняя неделя" , 0 },
                { "Нижняя неделя" , 0 }
            };
            if (week == null) Count2["Обе недели"]++;
            else Count2[week]++;
            Rooms.Add(room);
        }
    }
}
