namespace VKR_Schedule.Misc
{
    public class InfectedAuditoria
    {
        public string RoomNumber { get; set; }
        public DateTime StartInfectionDate { get; set; }
        public Dictionary<string, DateTime> ZeroPatientsData { get; set; }
        public bool _isInfectedNow;
        public List<string> InfectLecturers { get; set; }
        public List<string> InfectGroups { get; set; }

        public InfectedAuditoria(string roomNumber, DateTime dateStartInfection, string patientData)
        {
            RoomNumber = roomNumber;
            StartInfectionDate = dateStartInfection;
            ZeroPatientsData = new()
            {
                { patientData, dateStartInfection }
            };
            _isInfectedNow = true;
            InfectLecturers = new List<string>();
            InfectGroups = new List<string>();
        }

        public void EnterNewInfectionEvent(DateTime dateStartInfect, string patientData)
        {
            //если это новый оборот заражения - новый нулевой пациент
            if (!IsStillInfected(dateStartInfect))
            {
                if (!ZeroPatientsData.ContainsKey(patientData))
                    ZeroPatientsData.Add(patientData, dateStartInfect);
                else
                    ZeroPatientsData[patientData] = dateStartInfect;
            }
            StartInfectionDate = dateStartInfect;
            _isInfectedNow = true;
        }

        public void CheckIsStillInfected(DateTime curDate)
        {
            _isInfectedNow = IsStillInfected(curDate);
        }

        private bool IsStillInfected(DateTime curDate)
        {
            return curDate <= StartInfectionDate.AddDays(4);
        }
    }
}
