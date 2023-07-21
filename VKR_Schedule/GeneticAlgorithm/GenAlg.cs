using ScottPlot;
using System.Diagnostics;
using System.Windows.Forms;
using VKR_Schedule.DB_Models;
using VKR_Schedule.Misc;
using VKR_Schedule.Models;

namespace VKR_Schedule.GeneticAlgorithm
{
    internal class GenAlg
    {
        public List<Schedule> _schedules = new();
        private readonly Schedule _original;
        private readonly int _populationSize;
        private readonly float _mutationRate;
        private readonly int _maxGenerations;
        private readonly int _eliteCount;
        private readonly float _fitnessGoal;
        private readonly int[] timeSlots = new int[] { 8, 10, 11, 14, 15, 17, 19, 20 };

        readonly Random _random = new();

        public GenAlg(Schedule schedule, int populationSize, float mutationRate, int maxGenerations, float fitnessGoal, int eliteCount)
        {
            
            _original = schedule;
            _original.CalculateFitness(calcInf: false, 1, 0.016f, 0.016f, 10);

            //_schedules.Add(schedule);
            _populationSize = populationSize;
            _mutationRate = mutationRate;
            _maxGenerations = maxGenerations;
            _fitnessGoal = fitnessGoal;
            _eliteCount = eliteCount;
        }

        public Schedule Start(FormsPlot plot, bool calculateInf)
        {
            //_schedules[0].CalculateFitness(calcInf: calculateInf, 1, 0.016f, 0.016f, 10);
            //PrintScheduleData.Print(_schedules[0], true);
            //plot.Plot.AddHorizontalLine(_schedules[0].Fitness, label: "Оригинальное расписание");
            PrintScheduleData.Print(_original, true);
            plot.Plot.AddHorizontalLine(_original.Fitness, label: "Оригинальное расписание");

            InitializePopulation(_populationSize);
            int generation = 0;

            double[] dataX = DataGen.Consecutive(_maxGenerations + 1);
            double[] dataY = new double[_maxGenerations + 1];
            double[] dataZ = new double[_maxGenerations + 1];

            while (generation < _maxGenerations)
            {
                Parallel.ForEach(_schedules.Where(s => s.Fitness == 0), schedule =>
                {
                    schedule.CalculateFitness(calcInf: calculateInf, 1, 0.016f, 0.016f, 10);
                });

                Console.WriteLine(generation);

                dataY[generation] = _schedules.Sum(s => s.Fitness) / _populationSize;
                dataZ[generation] = _schedules.Max(s => s.Fitness);

                if (FoundSolution())
                {
                    return GetFittest();
                }

                List<Schedule> newPopulation = new();
                newPopulation.AddRange(GetFittest(_eliteCount));
                Parallel.For(_eliteCount, _populationSize, (i, state) =>
                {
                    Schedule parent1 = Selection();
                    Schedule parent2;
                    do parent2 = Selection();
                    while (parent1 == parent2);
                    Schedule offspring = Crossover(parent1, parent2);
                    Mutation(offspring);
                    offspring.StudentGroups.ForEach(s =>
                    {
                        s.CheckSchedule();
                    });
                    newPopulation.Add(offspring);
                });

                _schedules = newPopulation;
                generation++;

                if (generation == _maxGenerations)
                {
                    Parallel.ForEach(_schedules.Where(s => s.Fitness == 0), schedule =>
                    {
                        schedule.CalculateFitness(calcInf: calculateInf, 1, 0.016f, 0.016f, 10);
                    });
                    dataY[generation] = _schedules.Sum(s => s.Fitness) / _populationSize;
                    dataZ[generation] = _schedules.Max(s => s.Fitness);
                }
            }
            plot.Plot.AddScatter(dataX, dataY, label: "Средняя пригодность поколения");
            plot.Plot.AddScatter(dataX, dataZ, label: "Максимальная пригодность поколения");
            return GetFittest();
        }

        private void InitializePopulation(int populationSize)
        {
            while (_schedules.Count != populationSize)
            {
                //List<StudentGroup> studentGroups = new(_schedules[0].StudentGroups.Count);
                //foreach (var group in _schedules[0].StudentGroups)
                //{
                //    studentGroups.Add(new StudentGroup(group));
                //    studentGroups.Last().FillSchedule();
                //}
                //_schedules.Add(new Schedule(studentGroups));

                List<StudentGroup> studentGroups = new(_original.StudentGroups.Count);
                foreach (var group in _original.StudentGroups)
                {
                    studentGroups.Add(new StudentGroup(group));
                    studentGroups.Last().FillSchedule();
                }
                _schedules.Add(new Schedule(studentGroups));
            }
        }

        // Проверка на наличие решения с требуемой приспособленностью
        private bool FoundSolution()
        {
            return _schedules.Any(g => g.Fitness >= _fitnessGoal);
        }

        // Получение наиболее приспособленного индивида
        public Schedule GetFittest()
        {
            return _schedules.MaxBy(s => s.Fitness);
        }
        public List<Schedule> GetFittest(int n)
        {
            return _schedules.OrderByDescending(g => g.Fitness).Take(n).ToList();
        }

        // Селекция родителей
        private Schedule Selection()
        {
            float totalFitness = _schedules.Sum(g => g.Fitness);
            float randomValue = (float)(_random.NextDouble() * totalFitness);
            float cumulativeFitness = 0;

            foreach (var schedule in _schedules)
            {
                cumulativeFitness += schedule.Fitness;
                if (cumulativeFitness >= randomValue)
                {
                    return schedule;
                }
            }

            return null; // Не должно произойти, для предотвращения ошибки компиляции
        }

        // Кроссовер родителей для порождения новых потомков
        private Schedule Crossover(Schedule parent1, Schedule parent2)
        {
            int numStudentGroups = parent1.StudentGroups.Count;
            List<StudentGroup> studentGroups = new(numStudentGroups);
            for (int i = 0; i < numStudentGroups; i++)
            {
                studentGroups.Add(new StudentGroup(parent1.StudentGroups[i]));
                foreach (var dayOfWeek in Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList().Skip(1))
                {
                    int crossoverPoint = _random.Next(0, 8);
                    var timeslots1 = timeSlots.Take(crossoverPoint).ToList();
                    var timeslots2 = timeSlots.Skip(crossoverPoint).ToList();
                    List<ScheduleDayInfo> genesFromParent1 = new();
                    List<ScheduleDayInfo> genesFromParent2 = new();
                    timeslots1.ForEach(time =>
                    {
                        genesFromParent1.AddRange(parent1.StudentGroups[i].Schedule[dayOfWeek].Where(s => s.TimeSlot.StartHour == time).ToList().ConvertAll(x => x.Clone()));
                    });
                    timeslots2.ForEach(time =>
                    {
                        genesFromParent2.AddRange(parent2.StudentGroups[i].Schedule[dayOfWeek].Where(s => s.TimeSlot.StartHour == time).ToList().ConvertAll(x => x.Clone()));
                    });

                    studentGroups[i].Schedule[dayOfWeek] = genesFromParent1.Concat(genesFromParent2).ToList();
                }
            }

            return new Schedule(studentGroups);
        }

        // Мутация потомка
        private void Mutation(Schedule offspring)
        {
            foreach (var group in offspring.StudentGroups)
            {
                double mutationProb = _random.NextDouble();

                if (mutationProb <= _mutationRate)
                {
                    var randomDay = (DayOfWeek)_random.Next(1, 7);
                    int numberOfLessonsToSwap = group.Schedule[randomDay].Count >= 2 ? _random.Next(1, group.Schedule[randomDay].Count / 2) : 0;

                    List<int> genesToSwap = new();
                    for (int i = 0; i < numberOfLessonsToSwap; i++)
                    {
                        int gene1 = _random.Next(group.Schedule[randomDay].Count);
                        int gene2 = _random.Next(group.Schedule[randomDay].Count);
                        while (gene1 == gene2 || genesToSwap.Contains(gene1) || genesToSwap.Contains(gene2))
                        {
                            gene1 = _random.Next(group.Schedule[randomDay].Count);
                            gene2 = _random.Next(group.Schedule[randomDay].Count);
                        }
                        genesToSwap.Add(gene1);
                        genesToSwap.Add(gene2);
                    }

                    for (int i = 0; i < genesToSwap.Count; i += 2)
                    {
                        (group.Schedule[randomDay][genesToSwap[i + 1]].TimeSlot, group.Schedule[randomDay][genesToSwap[i]].TimeSlot) = (group.Schedule[randomDay][genesToSwap[i]].TimeSlot, group.Schedule[randomDay][genesToSwap[i + 1]].TimeSlot);
                    }
                }
            }
        }
    }
}
