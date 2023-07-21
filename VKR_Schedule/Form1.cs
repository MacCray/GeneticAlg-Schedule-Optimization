using Microsoft.Data.SqlClient;
using System.Windows.Forms;
//using VKR_Schedule.DB_Models;
using VKR_Schedule.GeneticAlgorithm;
using VKR_Schedule.Parsing;
using ScottPlot;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using VKR_Schedule.Misc;

namespace VKR_Schedule
{
    public partial class Form1 : Form
    {
        Schedule scheduleOriginal;
        List<Schedule> bestSchedules;
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form = new(scheduleOriginal, bestSchedules);
            form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            scheduleOriginal = DatabaseParsing.ParseSchedule();
            GenAlg genAlg = new(scheduleOriginal, populationSize: (int)popSize.Value, mutationRate: (float)mutProb.Value * 0.01f, maxGenerations: (int)genNum.Value, 1f, eliteCount: Convert.ToInt32((double)popSize.Value * 0.3));
            _ = genAlg.Start(formsPlot1, checkBox1.Checked);

            Console.WriteLine("Данные по расписаниям в последнем поколении:");
            foreach (var s in genAlg._schedules)
            {
                Console.Write($"Расписание {genAlg._schedules.IndexOf(s)}:\n\t");
                PrintScheduleData.Print(s);
            }

            bestSchedules = genAlg.GetFittest(3);
            Console.WriteLine("\nЛучшие расписания:");
            foreach (var s in bestSchedules)
            {
                //s.CalculateInfections();
                PrintScheduleData.Print(s);
            }

            formsPlot1.Plot.XAxis.Label("Поколение");
            formsPlot1.Plot.YAxis.Label("Пригодность");
            formsPlot1.Plot.XAxis2.Label("Изменение пригодности по ходу работы генетического алгоритма");
            formsPlot1.Plot.Legend();
            formsPlot1.Refresh();
        }
    }
}