// Author: Donald Chanthirath

using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace cslox
{
    public static class Lox
    {
        static bool hadError = false;
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                System.Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        /// <summary>
        /// Runs the lox file at the given path
        /// </summary>
        /// <param name="path"></param>
        private static void RunFile(String path)
        {
            //byte[] bytes = File.ReadAllBytes(path);
            //string content = Encoding.Default.GetString(bytes);

            string source = File.ReadAllText(path);
            Run(source);

            if (hadError) System.Environment.Exit(65);
        }

        /// <summary>
        /// Allows for the user to write lox code in the console
        /// </summary>
        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null) break;
                Run(line);
                hadError = false;
            }
        }

        /// <summary>
        /// So far prints out tokens
        /// </summary>
        /// <param name="source"></param>
        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        /// <summary>
        /// The main error reporting method
        /// </summary>
        /// <param name="line"></param>
        /// <param name="message"></param>
        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        /// <summary>
        /// The help method for reporting errors
        /// </summary>
        /// <param name="line"></param>
        /// <param name="where"></param>
        /// <param name="message"></param>
        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            hadError = true;
        }
    }
}
