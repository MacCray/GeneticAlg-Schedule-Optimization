using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VKR_Schedule.GeneticAlgorithm;

namespace VKR_Schedule
{
    public partial class Form2 : Form
    {
        readonly Schedule originalShedule;
        readonly List<Schedule> bestSchedules;
        public Form2(Schedule original, List<Schedule> scheduleList)
        {
            InitializeComponent();

            originalShedule = original;
            originalShedule.StudentGroups.ForEach(g =>
            {
                foreach (var day in g.Schedule)
                {
                    g.Schedule[day.Key] = day.Value.OrderBy(s => s.TimeSlot.StartHour).ThenBy(s => s.WeekType).ToList();
                }
            });
            bestSchedules = scheduleList;
            bestSchedules.ForEach(s =>
            {
                s.StudentGroups.ForEach(g =>
                {
                    foreach (var day in g.Schedule)
                    {
                        g.Schedule[day.Key] = day.Value.OrderBy(s => s.TimeSlot.StartHour).ThenBy(s => s.WeekType).ToList();
                    }
                });
            });

            Parallel.For(0, originalShedule.StudentGroups.Count, (i, state) =>
            {
                Parallel.ForEach(originalShedule.StudentGroups[i].Schedule, day =>
                {
                    Parallel.ForEach(bestSchedules, s =>
                    {
                        if (!s.StudentGroups[i].Schedule[day.Key].SequenceEqual(day.Value))
                        {
                            Console.WriteLine("Различие в расписании " + bestSchedules.IndexOf(s) + " по группе " + originalShedule.StudentGroups[i].GroupId + " в " + DateTimeFormatInfo.CurrentInfo.GetDayName(day.Key));
                        }
                    });
                });
            });
            comboBox1.DataSource = originalShedule.StudentGroups;
            comboBox2.Items.AddRange(bestSchedules.Select(s => $"Расписание {bestSchedules.IndexOf(s)} - Пригодность: {s.Fitness}").ToArray());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplaySchedules();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == null)
                comboBox1.SelectedIndex = 0;
            DisplaySchedules();
        }

        private void DisplaySchedules()
        {
            richTextBox1.Text = originalShedule.StudentGroups[comboBox1.SelectedIndex].PrintSchedule();
            richTextBox2.Text = bestSchedules[comboBox2.SelectedIndex].StudentGroups[comboBox1.SelectedIndex].PrintSchedule();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bestSchedules[comboBox2.SelectedIndex].StudentGroups[comboBox1.SelectedIndex].CheckSchedule();
        }
    }
}
