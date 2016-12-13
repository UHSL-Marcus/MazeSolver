using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeSolver;
using System.IO;
using System.Text.RegularExpressions;

namespace MazeSolverTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Building input commands...");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                sb.Append("0" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            }

            sb.Append("Q" + Environment.NewLine);
            Console.WriteLine("Built");

            using (var sw = new StringWriter())
            {
                using (var sr =  new StringReader(sb.ToString()))
                {
                    var oriOut = Console.Out;
                    var oriIn = Console.In;

                    Console.Write("Running program...");
                    Console.SetOut(sw);
                    Console.SetIn(sr);
                    MazeSolver.Program.Main(args);

                    Console.SetOut(oriOut);
                    Console.SetIn(oriIn);
                    Console.WriteLine("Complete");

                    Console.Write("Parsing output...");
                    // search for the result lines
                    long totalTicks = 0;
                    long totalMillis = 0;
                    int ticksCount = 0;
                    int millisCount = 0;

                    int total = 0;
                    using (StringReader osr = new StringReader(sw.ToString()))
                    {
                        // Loop over the lines in the string.
                        string line;
                        while ((line = osr.ReadLine()) != null)
                        {
                            if (Regex.IsMatch(line, "Maze solved in"))
                            {
                                total++;
                                MatchCollection matches = Regex.Matches(line, @"\d+");
                                if (matches.Count == 2)
                                {
                                    long ticks;
                                    long millis;
                                    if (long.TryParse(matches[0].Value, out ticks))
                                    {
                                        ticksCount++;
                                        totalTicks += ticks;
                                    }
                                    if (long.TryParse(matches[1].Value, out millis))
                                    {
                                        millisCount++;
                                        totalMillis += millis;
                                    }
                                }
                            }
                        }
                    }
                    Console.WriteLine("Complete");
                    


                    long aveTicks = totalTicks / ticksCount;
                    long aveMillis = totalMillis / millisCount;

                    Console.WriteLine("{0} Tests ran. {1} Average Ticks, {2} Average ms", total, aveTicks, aveMillis);
                }
            }
        }
    }


}
