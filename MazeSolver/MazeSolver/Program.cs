using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver
{
   public class Program
    {
        /// <summary>
        /// Holds the detected txt file paths
        /// </summary>
        static List<string> fileOptions = new List<string>();

        /// <summary>
        /// The entry point for the application.
        /// </summary>
        /// <param name="args"> A list of paths to search for txt files</param>
        public static void Main(string[] args)
        {
            // time the entire operation, pause while waiting for user input.
            Stopwatch stopwatch_overall = new Stopwatch();
            stopwatch_overall.Start();

            // through tests I found that the time taken to solve a maze reduces from 3k ticks to 8 ticks (for the small maze) after 2 runs of the alorithm.
            // this optimisation carries over to all susequent operations. So to give the program automatic runtime optisation, I run the solver over the small maze on startup.

            // NOTE: when running more than 1 or 2 tests, this is obviously negligable. But when starting up the program to only solve one or 2 large mazes, this seem to be a good trade off.
            // More is gained from the reduced time in the larger mazes than is lost running these 2 quick solves.

            // keep Maze local, no need for field lookups
            Maze maze;
            for (int opti = 0; opti < 2; opti++)
            {
                maze = new Maze();
                using (Stream s = Properties.Resources.optimisationMaze.ToStream())
                    maze.LoadMaze(s);

                maze.Solve();
            }

            // list all txt files in the given dir(s) as well as default (./Mazes).
            string options = "Available txt Files:\n";
            if (args.Length > 0)
            {
                // loop each passed dir path
                foreach (string path in args)
                {
                    options += DisplayOptions(path);
                }
            }
            // include the default
            options += DisplayOptions(Properties.Settings.Default.DefaultMazeDir);

            options += "Q: Exit\n";

            // continue if at least one txt file was detected. 
            if (fileOptions.Count > 0)
            {
                string input = "";
                while (!input.Equals("Q"))
                {
                    int idx;
                    Console.Write(options + "Pick Option: ");

                    bool success = false;
                    while (!success)
                    {

                        stopwatch_overall.Stop();
                        input = Console.ReadLine();
                        stopwatch_overall.Start();

                        success = input.Equals("Q");

                        string error = "Invalid input";

                        if (int.TryParse(input, out idx))
                        {
                            if (idx > -1 && idx < fileOptions.Count)
                            {
                                // valid input
                                error = "Invalid file";

                                maze = new Maze();
                                if (maze.LoadMazeFile(fileOptions[idx])) {
                                    // valid file
                                    success = true;

                                    // preview maze
                                    Console.Write(maze.ToString());
                                    Console.WriteLine("Maze loaded in {0} Ticks, {1}ms. Press any key to solve...", maze.getLoadTicks(), maze.getLoadMillis());
                                    stopwatch_overall.Stop();
                                    Console.ReadLine();
                                    stopwatch_overall.Start();

                                    // solve maze
                                    if (maze.Solve())
                                    {
                                        Console.WriteLine("Solved! Drawing result...");
                                        Console.Write(maze.ResultString());
                                        Console.WriteLine("Maze solved in {0} ticks, {1}ms", maze.getSolvedTicks(), maze.getSolvedMillis());
                                        Console.WriteLine("Maze loaded and solved in {0} ticks, {1}ms", maze.getSolvedTicks() + maze.getLoadTicks(), maze.getSolvedMillis() + maze.getLoadMillis());
                                    } else
                                    {
                                        Console.WriteLine("Unable to solve maze.");
                                    }
                                    stopwatch_overall.Stop();
                                    Console.WriteLine("Total time for entire operation: {0} ticks, {1}ms. Press any key to continue.", stopwatch_overall.ElapsedTicks, stopwatch_overall.ElapsedMilliseconds);
                                    Console.ReadLine();
                                }
                            }
                        }

                        if (!success)
                        {
                            ConsoleEx.ClearLine();
                            Console.Write(error + " try again: ");
                        }
                    }
                    
                }

            } else
            {
                Console.WriteLine("No maze files, press any key to exit...");
                Console.ReadLine();
            }

            return;

        }

        static string DisplayOptions(string dir)
        {
            string optionList = "";
            int startIdx = fileOptions.Count;

            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir, "*.txt");

                foreach (string file in files)
                {
                    if (File.Exists(file))
                    {
                        optionList += startIdx + ": " + file + "\n";
                        fileOptions.Insert(startIdx, file);
                        startIdx++;
                    }
                }
            }
            else
            {
                optionList += string.Format("{0} is not a valid directory\n", dir);
            }

            return optionList;
            
        }
    }
}
