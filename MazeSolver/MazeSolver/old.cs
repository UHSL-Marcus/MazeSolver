/*using System;
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
    class Program
    {
        static List<string> fileOptions = new List<string>();

        struct currentPoint
        {
            public int X;
            public int Y;
            public currentPoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        private class Maze
        {
            public int height;
            public int width;
            public int startX;
            public int startY;
            public int endX;
            public int endY;
            public BitArray mazeData;

        }
        static void Main(string[] args)
        {
            Stopwatch stopwatch_overall = new Stopwatch();
            stopwatch_overall.Start();


            // list all txt files in the given dir(s) or if none, search default (./Mazes).
            string options = "Available txt Files:\n";
            if (args.Length > 0)
            {
                foreach (string path in args)
                {
                    if (Directory.Exists(path))
                    {
                        options += DisplayOptions(path);
                    }
                    else
                    {
                        Console.WriteLine("{0} is not a valid directory", path);
                    }
                }
            }
            options += DisplayOptions(Properties.Settings.Default.DefaultMazeDir);

            if (fileOptions.Count > 0)
            {
                int height = 0;
                int width = 0;
                int startX = 0;
                int startY = 0;
                int endX = 0;
                int endY = 0;
                BitArray mazeData = new BitArray(1);
                int[] mazeDistance = new int[1];
                currentPoint[] mazeParents = new currentPoint[1];
                Stopwatch stopwatch = new Stopwatch();


                Console.Write(options + "Pick Option: ");
                bool success = false;
                while (!success)
                {
                    string error = "Invalid input";
                    int idx;
                    stopwatch_overall.Stop();
                    if (int.TryParse(Console.ReadLine(), out idx))
                    {
                        stopwatch_overall.Start();
                        if (idx > -1 && idx < fileOptions.Count)
                        {
                            error = "Invalid file";
                            StreamReader mazeFile = new StreamReader(File.OpenRead(fileOptions[idx]));


                            int[] dimentions = splitIntoInts(mazeFile.ReadLine(), ' ');

                            if (dimentions.Length == 2)
                            {
                                width = dimentions[0];
                                height = dimentions[1];

                                int[] start = splitIntoInts(mazeFile.ReadLine(), ' ');

                                if (start.Length == 2)
                                {
                                    startX = start[0];
                                    startY = start[1];

                                    int[] end = splitIntoInts(mazeFile.ReadLine(), ' ');

                                    if (end.Length == 2)
                                    {
                                        endX = end[0];
                                        endY = end[1];

                                        mazeData = new BitArray(height * width);
                                        mazeDistance = new int[height * width];
                                        mazeParents = new currentPoint[height * width];

                                        success = true;
                                        for (int i = 0; i < height; i++)
                                        {
                                            if (!mazeFile.EndOfStream)
                                            {
                                                bool[] row = splitIntoBools(mazeFile.ReadLine(), ' ');
                                                if (row.Length == width)
                                                {
                                                    for (int b = 0; b < row.Length; b++)
                                                    {
                                                        mazeData[b + (width * i)] = row[b];
                                                    }
                                                }
                                                else success = false;
                                            }
                                            else success = false;

                                        }
                                    }
                                }
                            }

                        }
                    }

                    if (!success)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.Write(error + " try again: ");
                    }
                }

                string mazePrint = "***Loaded Maze***\n" +
                    "Height: " + height + ", Width: " + width + "\n" +
                    "Start: (" + startX + ", " + startY + ")\n" +
                    "End: (" + endX + ", " + endY + ")\n" +
                    "Maze: \n";

                for (int y = 0; y < height; y++)
                {

                    for (int x = 0; x < width; x++)
                    {
                        mazePrint += (mazeData[x + (y * width)] ? 1 : 0) + " ";
                    }
                    mazePrint += "\n";
                }
                Console.Write(mazePrint + "Press any key to solve...");
                stopwatch_overall.Stop();
                Console.ReadLine();
                stopwatch_overall.Start();

                stopwatch.Start();

                Queue<currentPoint> todo = new Queue<currentPoint>();
                todo.Enqueue(new currentPoint(startX, startY));
                mazeDistance[startX + (startY * width)] = 1;

                bool endFound = false;
                while (todo.Count > 0 && !endFound)
                {
                    currentPoint p = todo.Dequeue();
                    int pX = p.X;
                    int pY = p.Y;

                    if (pX == endX && pY == endY) endFound = true;
                    else
                    {

                        int current = mazeDistance[pX + (pY * width)];
                        int newX = pX + 1;
                        int newY = pY;
                        if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                        {
                            if (mazeDistance[newX + (newY * width)] == 0) // not yet processed
                            {
                                if (!mazeData[newX + (newY * width)]) // viable path
                                {
                                    mazeDistance[newX + (newY * width)] = current + 1;
                                    mazeParents[newX + (newY * width)] = p;
                                    todo.Enqueue(new currentPoint(newX, newY));
                                }
                            }
                        }

                        newX = pX - 1;
                        newY = pY;
                        if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                        {
                            if (mazeDistance[newX + (newY * width)] == 0) // not yet processed
                            {
                                if (!mazeData[newX + (newY * width)]) // viable path
                                {
                                    mazeDistance[newX + (newY * width)] = current + 1;
                                    mazeParents[newX + (newY * width)] = p;
                                    todo.Enqueue(new currentPoint(newX, newY));
                                }
                            }
                        }

                        newX = pX;
                        newY = pY + 1;
                        if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                        {
                            if (mazeDistance[newX + (newY * width)] == 0) // not yet processed
                            {
                                if (!mazeData[newX + (newY * width)]) // viable path
                                {
                                    mazeDistance[newX + (newY * width)] = current + 1;
                                    mazeParents[newX + (newY * width)] = p;
                                    todo.Enqueue(new currentPoint(newX, newY));
                                }
                            }
                        }

                        newX = pX;
                        newY = pY - 1;
                        if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                        {
                            if (mazeDistance[newX + (newY * width)] == 0) // not yet processed
                            {
                                if (!mazeData[newX + (newY * width)]) // viable path
                                {
                                    mazeDistance[newX + (newY * width)] = current + 1;
                                    mazeParents[newX + (newY * width)] = p;
                                    todo.Enqueue(new currentPoint(newX, newY));
                                }
                            }
                        }
                    }
                }

                stopwatch.Stop();
                Console.WriteLine("Maze Solved in " + stopwatch.ElapsedMilliseconds + "ms");
                Console.WriteLine("Drawing Result");

                char[] mazeDrawing = new char[height * width];
                // fill in start and end
                mazeDrawing[startX + (startY * width)] = 'S';
                mazeDrawing[endX + (endY * width)] = 'E';
                // loop through each parent, add an X until we find start
                Queue<currentPoint> path = new Queue<currentPoint>();
                // start with end point
                path.Enqueue(new currentPoint(endX, endY));

                while (path.Count > 0)
                {
                    currentPoint p = path.Dequeue();
                    if (p.X != startX || p.Y != startY)
                    {
                        currentPoint next = mazeParents[p.X + (p.Y * width)];
                        path.Enqueue(mazeParents[p.X + (p.Y * width)]);
                        if (p.X != endX || p.Y != endY)
                        {
                            mazeDrawing[p.X + (p.Y * width)] = 'X';
                        }
                    }
                }

                // fill in the rest
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        char curr = mazeDrawing[x + (y * width)];
                        if (mazeDrawing[x + (y * width)] == '\0')
                        {
                            if (mazeData[x + (y * width)])
                                mazeDrawing[x + (y * width)] = '#';
                            else mazeDrawing[x + (y * width)] = ' ';
                        }
                    }
                }

                // print it out
                string solvedPrint = "";
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        solvedPrint += mazeDrawing[x + (y * width)] + " ";
                    }
                    solvedPrint += "\n";
                }
                Console.Write(solvedPrint);
                stopwatch_overall.Stop();
                Console.WriteLine("Total time for entire operation: " + stopwatch_overall.ElapsedMilliseconds + "ms");
                Console.ReadLine();

            }
            else
            {
                Console.WriteLine("No maze files, press any key to exit...");
                Console.ReadLine();
                return;
            }


        }

        static bool isInMaze(int x, int y, int height, int width)
        {
            return (x >= 0 && x < width && y >= 0 && y < height);
        }

        static string DisplayOptions(string dir)
        {
            string optionList = "";
            int startIdx = fileOptions.Count;

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

            return optionList;

        }

        static int[] splitIntoInts(string input, char delim)
        {
            string[] split = input.Split(delim);
            int[] output = new int[split.Length];
            for (int i = 0; i < split.Length; i++)
                int.TryParse(split[i], out output[i]);

            return output;
        }

        static bool[] splitIntoBools(string input, char delim)
        {
            string[] split = input.Split(delim);
            bool[] output = new bool[split.Length];
            for (int i = 0; i < split.Length; i++)
            {
                int entry;
                if (int.TryParse(split[i], out entry))
                {
                    output[i] = (entry == 1);
                }
            }
            return output;
        }
    }
}*/
