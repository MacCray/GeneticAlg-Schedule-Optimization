namespace VKR_Schedule.Models
{
    public class Room
    {
        public string RoomNumber { get; set; }
        public string Building { get; set; }

        public Room(string roomNumber, string building)
        {
            RoomNumber = roomNumber;
            Building = building;
        }
    }
}