namespace VKR_Schedule.Models
{
    public class TimeSlot
    {
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }

        public TimeSlot(string timePeriod)
        {
            StartHour = int.Parse(timePeriod[..2]);
            StartMinute = int.Parse(timePeriod[3..5]);
            EndHour = int.Parse(timePeriod[6..8]);
            EndMinute = int.Parse(timePeriod[9..]);
        }
        public TimeSlot(int startHour, int startMinute)
        {
            StartHour = startHour;
            StartMinute = startMinute;
            EndHour = startHour + 1;
            EndMinute = startMinute + 30;
            if (EndMinute >= 60)
            {
                EndHour += 1;
                EndMinute -= 60;
            }
        }
        public override string ToString()
        {
            return $"{StartHour}:{(StartMinute == 0 ? "00" : StartMinute)}-{EndHour}:{(EndMinute == 0 ? "00" : EndMinute)}";
        }
    }
}