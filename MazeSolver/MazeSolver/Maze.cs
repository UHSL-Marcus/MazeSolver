using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver
{
    /// <summary>
    /// Maze solver class, loads maze data from a specific file type and finds the shortest route from start to end.
    /// </summary>
    class Maze
    {
        /// <summary>
        /// simple structure to hold X and Y coords
        /// </summary>
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

        // Store all the loaded maze info
        private int height = 0;
        private int width = 0;
        private int startX = 0;
        private int startY = 0;
        private int endX = 0;
        private int endY = 0;

        // smallest way to store 1's and 0's
        private BitArray mazeData;

        // simple arrays have less overhead than Lists etc
        // stores the x,y position's distance from start
        private int[] mazeDistance;
        // stores the parent of the x,y position
        private currentPoint[] mazeParents;

        // Timing the solving operation
        private Stopwatch stopwatch = new Stopwatch();
        private long loadOpTicks = 0;
        private long loadOpMillis = 0;

        private long solveTicks = 0;
        private long solveMillis = 0;

        bool Loaded = false;

        /// <summary>
        /// Load a maze txt file
        /// </summary>
        /// <param name="file">The file path</param>
        /// <seealso cref="string"></seealso>
        /// <returns>
        /// Returns whether the maze was loaded successfully. True or false.
        /// </returns>
        public bool LoadMazeFile(string file)
        {
            using (FileStream fs = File.OpenRead(file))
                return LoadMaze(fs);
        }

        /// <summary>
        /// Load a maze from a data stream
        /// </summary>
        /// <param name="mazeInfo">The maze data stream</param>
        /// <seealso cref="Stream"></seealso>
        /// <returns>
        /// Returns whether the maze was loaded successfully. True or false.
        /// </returns>
        public bool LoadMaze(Stream mazeInfo)
        {
            stopwatch.Restart();

            bool success = false;

            StreamReader mazeFile = new StreamReader(mazeInfo);

            // The spec says files will always be well formed. However something could go wrong in loading.

            // read each line and pull out the information required.
            int[] dimentions = mazeFile.ReadLine().SplitInt(' ');

            // height and width
            if (dimentions.Length == 2)
            {
                width = dimentions[0];
                height = dimentions[1];

                int[] start = mazeFile.ReadLine().SplitInt(' ');

                // start coords
                if (start.Length == 2)
                {
                    startX = start[0];
                    startY = start[1];

                    int[] end = mazeFile.ReadLine().SplitInt(' ');

                    // end coords
                    if (end.Length == 2)
                    {
                        endX = end[0];
                        endY = end[1];

                        // the maze data
                        mazeData = new BitArray(height * width);
                        mazeDistance = new int[height * width];
                        mazeParents = new currentPoint[height * width];

                        success = true;
                        for (int i = 0; i < height; i++)
                        {
                            if (!mazeFile.EndOfStream)
                            {
                                bool[] row = mazeFile.ReadLine().SplitBool(' ');
                                if (row.Length == width)
                                {
                                    for (int b = 0; b < row.Length; b++)
                                    {
                                        mazeData[b + (width * i)] = row[b];
                                    }
                                }
                                else success = false; // the data row is not the correct length
                            }
                            else success = false; // hit end of stream early, maze height does not match data. 

                        }
                    }
                }
                
            }

            stopwatch.Stop();

            loadOpMillis = stopwatch.ElapsedMilliseconds;
            loadOpTicks = stopwatch.ElapsedTicks;

            Loaded = success;
            return success;
        }

        delegate void CheckPoint(int x, int y);
        /// <summary>
        /// Solve the maze using breadth first search
        /// </summary>
        /// <returns>
        /// Returns whether the maze was solved successfully. True or false.
        /// </returns>
        /// 
        public bool Solve()
        {
            // tested copying the fields to local variables here, it gave no signficat benefit to the speed of the algorithm when averaging 1000 large maze solves.
            stopwatch.Restart();

            // start with the starting point
            Queue<currentPoint> todo = new Queue<currentPoint>();
            todo.Enqueue(new currentPoint(startX, startY));

            // using 'width' as a stride to separate data into rows when accessing.
            // use 1 rather than 0, just to save time inialising all the entries as -1, since 0 is the int default. 
            mazeDistance[startX + (startY * width)] = 1;

            // solved flag
            bool endFound = false;

            // loop until all traversable points have been searched, or end has been found.
            while (todo.Count > 0 && !endFound)
            {
                // grab the current point
                currentPoint p = todo.Dequeue();
                // local variables rather than pulling the object refrence each time. 
                int pX = p.X;
                int pY = p.Y;

                // check for end location
                if (pX == endX && pY == endY) endFound = true;
                else
                {
                    // get the current disance from start
                    int current = mazeDistance[pX + (pY * width)];

                    // check the adjacent points
                    // use an inline method as this is not strictly a "class" funtion,
                    // but rather a section of repeated code only used by this method.
                    CheckPoint checkPoint = (x, y) => {
                        // make sure point is within the bounds of the maze
                        if (x >= 0 && x < width && y >= 0 && y < height)
                        {
                            if (mazeDistance[x + (y * width)] == 0) // not yet processed
                            {
                                if (!mazeData[x + (y * width)]) // only process viable pathways
                                {
                                    // add distance, parent and queue for further processing
                                    mazeDistance[x + (y * width)] = current + 1;
                                    mazeParents[x + (y * width)] = p;
                                    todo.Enqueue(new currentPoint(x, y));
                                }
                            }
                        }
                    };

                    // Right
                    int newX = pX + 1;
                    int newY = pY;
                    checkPoint(newX, newY);

                    // Left
                    newX = pX - 1;
                    newY = pY;
                    checkPoint(newX, newY);

                    // Up
                    newX = pX;
                    newY = pY + 1;
                    checkPoint(newX, newY);

                    // Down
                    newX = pX;
                    newY = pY - 1;
                    checkPoint(newX, newY);
                }
            }
            stopwatch.Stop();

            solveMillis = stopwatch.ElapsedMilliseconds;
            solveTicks = stopwatch.ElapsedTicks;

            return endFound;
        }

        /// <summary>
        /// String showing the solved maze result
        /// </summary>
        /// <returns>
        /// Returns string representation of the solved maze. 
        /// </returns>
        /// 
        public string ResultString()
        {
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
                //  while not reaching the start
                if (p.X != startX || p.Y != startY)
                {
                    // queue this points parent
                    path.Enqueue(mazeParents[p.X + (p.Y * width)]);
                    if (p.X != endX || p.Y != endY)
                    {
                        // draw an X if this is not the end location
                        mazeDrawing[p.X + (p.Y * width)] = 'X';
                    }
                }
            }

            // fill in the rest
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // if the point has not already been filled in
                    if (mazeDrawing[x + (y * width)] == '\0')
                    {
                        // wall or pathway
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
            return solvedPrint;
        }

        /// <summary>
        /// Get the number of processor timer ticks to solve maze
        /// </summary>
        /// <returns>
        /// Returns long
        /// </returns>
        public long getSolvedTicks()
        {
            return solveTicks;
        }

        /// <summary>
        /// Get the number of milliseconds taken to solve maze
        /// </summary>
        /// <returns>
        /// Returns long
        /// </returns>
        public long getSolvedMillis()
        {
            return solveMillis;
        }

        /// <summary>
        /// Get the number of processor timer ticks to load maze
        /// </summary>
        /// <returns>
        /// Returns long
        /// </returns>
        public long getLoadTicks()
        {
            return loadOpTicks;
        }

        /// <summary>
        /// Get the number of milliseconds taken to load maze
        /// </summary>
        /// <returns>
        /// Returns long
        /// </returns>
        public long getLoadMillis()
        {
            return loadOpMillis;
        }

        /// <summary>
        /// Maze input details as a string
        /// </summary>
        /// <returns>
        /// Returns string representation of the maze input. 
        /// </returns>
        public override string ToString()
        {
            string mazePrint = "Height: " + height + ", Width: " + width + "\n" +
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

            return mazePrint;
        }
    }

}
