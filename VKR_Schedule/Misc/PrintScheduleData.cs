using VKR_Schedule.GeneticAlgorithm;
using VKR_Schedule.Models;

namespace VKR_Schedule.Misc
{
    internal static class PrintScheduleData
    {
        public static void Print(Schedule schedule, bool scheduleOriginal = false)
        {
            if (scheduleOriginal)
                Console.WriteLine("Данные по оригинальному расписанию:");
            Console.WriteLine($"Пригодность - {schedule.Fitness}; "
                              + $"Штрафы за конфликты - {schedule.Conflicts}; "
                              + $"Штрафы за перерывы - {schedule.Breaks}; "
                              + $"Штрафы за переезды - {schedule.Transfer}; "
                              + $"Штрафы за заражения - {schedule.Infections};");
        }
    }
}
