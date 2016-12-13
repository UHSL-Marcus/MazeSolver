using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver
{
    static class Extentions
    {
        /// <summary>
        /// Split a string into int tokens
        /// </summary>
        /// <param name="input"> The string to split</param>
        /// <seealso cref="string"></seealso>
        /// <param name="delim"> The delim character</param>
        /// <seealso cref="char"></seealso>
        /// <returns>
        /// Returns an array of int, containing all successfuly parsed ints.
        /// </returns>
        public static int[] SplitInt(this string input, char delim)
        {
            string[] split = input.Split(delim);
            int[] output = new int[split.Length];
            for (int i = 0; i < split.Length; i++)
                int.TryParse(split[i], out output[i]);

            return output;
        }

        /// <summary>
        /// Split a string into bool tokens, only works with "0" or "1"
        /// </summary>
        /// <param name="input"> The string to split</param>
        /// <seealso cref="string"></seealso>
        /// <param name="delim"> The delim character</param>
        /// <seealso cref="char"></seealso>
        /// <returns>
        /// Returns an array of bool, containing all successfuly parsed bools.
        /// </returns>
        public static bool[] SplitBool(this string input, char delim)
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

        /// <summary>
        /// Generates a stream from a string object
        /// </summary>
        /// <param name="input"> The input string</param>
        /// <seealso cref="string"></seealso>
        /// <returns>
        /// Returns a stream containing the string data
        /// </returns>
        public static Stream ToStream(this string input)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(input);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }

    /// <summary>
    /// Contains custom extention methods for the console
    /// </summary>
    public static class ConsoleEx
    {
        /// <summary>
        /// Clears the previously written line
        /// </summary>
        public static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
    }
}
