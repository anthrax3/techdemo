using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace dotnet_massive_async
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int numTasks = int.Parse(args[0]);
            TimeSpan sleepDelay = TimeSpan.FromSeconds(double.Parse(args[1]));
            long maxHits = long.Parse(args[2]);

            RuntimeHelper.ConfigureThreads();

            var scoreboard = new Scoreboard();
            for (var i = 0; i < numTasks; i++) {
                var task = WorkTask(scoreboard, sleepDelay);
            }
            await scoreboard.PollScores(maxHits);
        }

        static async Task WorkTask(Scoreboard scoreboard, TimeSpan sleepDelay)
        {
            while (true) {
                await Task.Delay(sleepDelay);
                scoreboard.AddHit();
            }
        }
    }


    class RuntimeHelper
    {
        public static void ConfigureThreads()
        {
            // FIXME: set from env
            //ThreadPool.SetMaxThreads(2, 2);

            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxIOThreads);
            ThreadPool.GetMinThreads(out int minWorkerThreads, out int minIOThreads);
            Console.WriteLine($"MinThreads: {minWorkerThreads}, {minIOThreads}");
            Console.WriteLine($"MaxThreads: {maxWorkerThreads}, {maxIOThreads}");
        }

        // Show current mem usage.  Useful for showing what the GC is doing.
        // /usr/bin/time -v just shows the max usage, which doesn't show the whole
        // picture for a generational GC.
        public static void PrintMemInfo() 
        {
            var lines = File.ReadAllText("/proc/self/status").Split('\n');
            foreach (var line in lines) {
                if (line.StartsWith("VmSize:") || line.StartsWith("VmRSS")) {
                    Console.WriteLine(" - " + line);
                }
            }
        }
    }


    class Scoreboard
    {
        public Scoreboard() { }

        public void AddHit() 
        {
            Interlocked.Increment(ref hitCount);
        }

        public async Task PollScores(long maxHits) 
        {
            while (true) {
                await Task.Delay(TimeSpan.FromSeconds(1.0));
                DateTime currentTime = DateTime.UtcNow;
                TimeSpan elapsed = currentTime - lastDumpTime;
                long totalHits = Interlocked.Read(ref hitCount);

                Console.WriteLine($"elapsed={elapsed} totalHits={totalHits}");
                RuntimeHelper.PrintMemInfo();
                lastDumpTime = currentTime;

                if (totalHits >= maxHits) {
                    Environment.Exit(0);
                }
            }
        }

        DateTime lastDumpTime = DateTime.UtcNow;
        long hitCount = 0;
    }
}
