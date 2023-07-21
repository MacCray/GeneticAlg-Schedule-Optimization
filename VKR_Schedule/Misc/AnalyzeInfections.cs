using System.Globalization;
using VKR_Schedule.GeneticAlgorithm;

namespace VKR_Schedule.Misc
{
    public class AnalyzeInfections
    {
        //входные данные
        public DateTime DateStartAnalize { get; set; }
        public DateTime DateStopInfect { get; set; }
        public DateTime DateStopAnalize { get; set; }
        public List<InfectedAuditoria> InfectedAuditorias { get; set; }
        public List<StudentGroup> InfectedGroups { get; set; }
        public List<string> InfectedLecturers { get; set; }
        public List<StudentGroup> AllGroups { get; set; }

        public AnalyzeInfections(DateTime DateSymptom, DateTime DateStopInfect, DateTime DateStopAnalize, StudentGroup group, List<StudentGroup> allGroups)
        {
            DateStartAnalize = DateSymptom;
            this.DateStopInfect = DateStopInfect;
            this.DateStopAnalize = DateStopAnalize;

            InfectedAuditorias = new List<InfectedAuditoria>();
            InfectedGroups = new List<StudentGroup>() { group };
            InfectedLecturers = new List<string>();
            AllGroups = allGroups;
        }

        public int MakeResearch()
        {
            var audDistant = "ЦДО Дистанционно";
            var audForeignLang = "каф.ИЯ";
            var audMilitary = "каф.ФВ";
            var noLecturer = "";
            var timeSlots = new int[] { 510, 610, 710, 840, 940, 1040, 1140, 1240 };
            List<InfectedAuditoria> stillInfectedAuditorias = new();

            //начинаем ежедневный мониторинг распространения и фиксирование информации
            for (DateTime analizeDay = DateStartAnalize; analizeDay <= DateStopAnalize; analizeDay = analizeDay.AddDays(1))
            {
                //вышел ли нулевой пациент на карантин
                if (analizeDay >= DateStopInfect)
                    ZeroPatientStopsInfect();

                //информация по текущему дню
                var dayOfWeek = analizeDay.DayOfWeek;
                string typeOfWeek = GetTypeOfWeek(analizeDay);

                if (dayOfWeek != DayOfWeek.Sunday)
                {
                    foreach (var time in timeSlots)
                    {
                        //ищем какие аудитории заражены от групп
                        CheckNewInfectByGroup(noLecturer, audDistant, audForeignLang, audMilitary, dayOfWeek, time, typeOfWeek, analizeDay);
                        //ищем какие аудитории заражены от преподавателей
                        CheckNewInfectByLecturer(noLecturer, audDistant, audForeignLang, audMilitary, dayOfWeek, time, typeOfWeek, analizeDay);
                        //ищем, кто заразился от зараженной аудитории
                        CheckNewInfectByAuditoria(ref stillInfectedAuditorias, noLecturer, audDistant, audForeignLang, audMilitary, dayOfWeek, time, typeOfWeek, analizeDay);
                    }
                }
            }
            return InfectedAuditorias.Count;
        }

        #region Функции проверок данных для грамотного проведения анализа

        private void ZeroPatientStopsInfect()
        {
            InfectedGroups.Remove(InfectedGroups.First());
        }
        #endregion

        #region Методы внутри алгоритма Анализа

        private void CheckNewInfectByGroup(string noLecturer, string audDistant, string audForeignLang, string audMilitary, DayOfWeek day, int time, string typeOfWeek, DateTime analizeDay)
        {
            //ищем какие аудитории заражены от группы
            foreach (var group in InfectedGroups)
            {
                var classes = group.Schedule[day].Where(
                    s =>
                        s.Room.RoomNumber != audDistant
                        && !s.Room.RoomNumber.Contains(audForeignLang)
                        && !s.Room.RoomNumber.Contains(audMilitary)
                        && s.TimeSlot.StartHour == time / 60
                        && s.Instructor != noLecturer
                        && (s.WeekType == null || s.WeekType == typeOfWeek)
                        ).ToList();
                //для каждой записи в расписании у зараженной группы - добавляем данные о новой инфицированной аудитории
                foreach (var cl in classes)
                    UpdateNewInfectedAuditoria(cl, analizeDay, group.GroupId);
            }
        }

        private void CheckNewInfectByLecturer(string noLecturer, string audDistant, string audForeignLang, string audMilitary, DayOfWeek day, int time, string typeOfWeek, DateTime analizeDay)
        {
            //ищем какие аудитории заражены от преподавателя
            foreach (var lecturer in InfectedLecturers)
            {
                var classes = AllGroups.SelectMany(s => s.Schedule).SelectMany(s => s.Value).Where(
                    s =>
                    s.Room.RoomNumber != audDistant
                    && !s.Room.RoomNumber.Contains(audForeignLang)
                    && !s.Room.RoomNumber.Contains(audMilitary)
                    && s.Instructor == lecturer && s.Instructor != noLecturer
                    && s.Day == day
                    && s.TimeSlot.StartHour == time / 60
                    && (s.WeekType == null || s.WeekType == typeOfWeek)
                    ).ToList();
                //для каждой записи в расписании у зараженной группы - обновляем данные об инфицированной аудитории
                foreach (var cl in classes)
                    UpdateNewInfectedAuditoria(cl, analizeDay, lecturer);
            }
        }

        private void CheckNewInfectByAuditoria(ref List<InfectedAuditoria> stillInfectedAuditorias, string noLecturer, string audDistant, string audForeignLang, string audMilitary, DayOfWeek day, int time, string typeOfWeek, DateTime analizeDay)
        {
            // проверяем не прошло ли 4 дня без инфицирования у зараженных аудиторий - меняем поле
            foreach (InfectedAuditoria ia in InfectedAuditorias)
            {
                ia.CheckIsStillInfected(analizeDay);
            }
            stillInfectedAuditorias = InfectedAuditorias.Where(x => x._isInfectedNow == true).ToList();

            //ищем, кто заразился сидя в зараженной аудитории
            foreach (InfectedAuditoria ia in stillInfectedAuditorias)
            {
                var classes = AllGroups.SelectMany(s => s.Schedule).SelectMany(s => s.Value).Where(
                    s =>
                    s.Room.RoomNumber == ia.RoomNumber
                    && s.Room.RoomNumber != audDistant
                    && !s.Room.RoomNumber.Contains(audForeignLang)
                    && !s.Room.RoomNumber.Contains(audMilitary)
                    && s.Day == day
                    && s.Instructor != noLecturer 
                    && s.TimeSlot.StartHour == time / 60 
                    && (s.WeekType == null || s.WeekType == typeOfWeek)
                    ).ToList();

                foreach (var cl in classes)
                {
                    if (InfectedLecturers.FirstOrDefault(x => x == cl.Instructor) == null)
                        InfectedLecturers.Add(cl.Instructor);
                    if (InfectedGroups.FirstOrDefault(x => x.GroupId == cl.GroupId) == null)
                        InfectedGroups.Add(AllGroups.First(g => g.GroupId == cl.GroupId));
                }
            }
        }

        private void UpdateNewInfectedAuditoria(ScheduleDayInfo cl, DateTime analizeDay, string patientData)
        {
            InfectedAuditoria iaSearched = InfectedAuditorias.FirstOrDefault(ia => ia.RoomNumber == cl.Room.RoomNumber);
            if (iaSearched is null)
                InfectedAuditorias.Add(new InfectedAuditoria(cl.Room.RoomNumber, analizeDay, patientData));
            else
                iaSearched.EnterNewInfectionEvent(analizeDay, patientData);
        }
        #endregion

        static string GetTypeOfWeek(DateTime day)
        {
            CultureInfo ci = new("ru-RU");
            Calendar cal = ci.Calendar;
            CalendarWeekRule cwr = ci.DateTimeFormat.CalendarWeekRule;
            DayOfWeek dow = ci.DateTimeFormat.FirstDayOfWeek;
            int weekNum = cal.GetWeekOfYear(day, cwr, dow);
            string type = weekNum % 2 == 0 ? "Верхняя неделя" : "Нижняя неделя";
            return type;
        }
    }
}