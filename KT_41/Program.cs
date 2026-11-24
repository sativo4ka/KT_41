using System.Diagnostics;

namespace KT_41
{
    internal class Program
    {
        static List<byte[]> longLivedObjects = new List<byte[]>();
        static List<byte[]> heavyLohObjects = new List<byte[]>();
        static void Main(string[] args)
        {
            Console.Title = "Симуляция нагрузки на сборщик мусора";

            Console.WriteLine("Симуляция нагрузки на сборщик мусора GC");
            Console.WriteLine("Программа демонстрирует три типа нагрузки:");
            Console.WriteLine("1. Короткоживущие объекты");
            Console.WriteLine("2. Долгоживущие объекты");
            Console.WriteLine("3. Большие объекты LOH");
            Console.WriteLine();

            Console.Write("Нажмите Enter, чтобы начать эксперимент...");
            Console.ReadLine();

            List<Result> results = new List<Result>();

            results.Add(RunTest("ShortLived", ShortLived));
            results.Add(RunTest("LongLived", LongLived));
            results.Add(RunTest("HeavyLoh", HeavyLoh));

            Console.WriteLine();
            Console.WriteLine("Принудительная сборка мусора GC.Collect");

            Console.WriteLine("Перед сборкой:");
            PrintGCCounts();

            List<byte[]> garbage = new List<byte[]>();

            for (int i = 0; i < 1000; i++)
            {
                garbage.Add(new byte[100000]);
            }

            garbage.Clear();

            Stopwatch gcStopwatch = Stopwatch.StartNew();

            GC.Collect(2, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            gcStopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine("После сборки:");
            PrintGCCounts();

            Console.WriteLine();
            Console.WriteLine("Время принудительной сборки: "
                + gcStopwatch.Elapsed.TotalMilliseconds.ToString("F3") + " мс");

            Console.WriteLine();
            Console.WriteLine("После сборки:");
            PrintGCCounts();

            Console.WriteLine();
            Console.WriteLine("Время принудительной сборки: "
                + gcStopwatch.Elapsed.TotalMilliseconds.ToString("F3") + " мс");

            PrintResults(results);

            Console.WriteLine();
            Console.WriteLine("Эксперимент завершен.");
            Console.ReadLine();
        }

        static Result RunTest(string name, Action method)
        {
            Console.WriteLine();
            Console.WriteLine("Запуск " + name);

            int gen0Before = GC.CollectionCount(0);
            int gen1Before = GC.CollectionCount(1);
            int gen2Before = GC.CollectionCount(2);

            Stopwatch stopwatch = Stopwatch.StartNew();

            method();

            stopwatch.Stop();

            int gen0After = GC.CollectionCount(0);
            int gen1After = GC.CollectionCount(1);
            int gen2After = GC.CollectionCount(2);

            int gen0 = gen0After - gen0Before;
            int gen1 = gen1After - gen1Before;
            int gen2 = gen2After - gen2Before;

            Console.WriteLine("Время выполнения: "
                + stopwatch.Elapsed.TotalMilliseconds.ToString("F3") + " мс");

            Console.WriteLine("Сборки Gen0: " + gen0);
            Console.WriteLine("Сборки Gen1: " + gen1);
            Console.WriteLine("Сборки Gen2: " + gen2);

            return new Result(name, stopwatch.Elapsed.TotalMilliseconds, gen0, gen1, gen2);
        }

        static void ShortLived()
        {
            for (int i = 0; i < 300000; i++)
            {
                byte[] data = new byte[200];
            }
        }

        static void LongLived()
        {
            for (int i = 0; i < 5000; i++)
            {
                longLivedObjects.Add(new byte[512]);
            }

            for (int i = 0; i < 100000; i++)
            {
                byte[] temp = new byte[200];
            }

            GC.Collect(2, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();

            GC.Collect(2, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
        }

        static void HeavyLoh()
        {
            for (int i = 0; i < 20; i++)
            {
                heavyLohObjects.Add(new byte[100000]);
            }

            for (int i = 0; i < 100; i++)
            {
                byte[] temp = new byte[100000];
            }

            GC.Collect(2, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
        }

        static void PrintGCCounts()
        {
            Console.WriteLine("Gen0: " + GC.CollectionCount(0));
            Console.WriteLine("Gen1: " + GC.CollectionCount(1));
            Console.WriteLine("Gen2: " + GC.CollectionCount(2));
        }

        static void PrintResults(List<Result> results)
        {
            Console.WriteLine();
            Console.WriteLine("Результаты эксперимента");

            Console.WriteLine(
                "{0,-15} {1,-15} {2,-10} {3,-10} {4,-10}",
                "Метод", "Время(мс)", "Gen0", "Gen1", "Gen2");

            foreach (Result r in results)
            {
                Console.WriteLine(
                    "{0,-15} {1,-15:F3} {2,-10} {3,-10} {4,-10}",
                    r.Name, r.TimeMs, r.Gen0, r.Gen1, r.Gen2);
            }
        }
    }
}