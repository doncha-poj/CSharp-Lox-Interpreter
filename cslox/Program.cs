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
            Console.WriteLine("Hello, World!");
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

        private static void RunFile(String path)
        {
            //byte[] bytes = File.ReadAllBytes(path);
            //string content = Encoding.Default.GetString(bytes);

            string source = File.ReadAllText(path);
            Run(source);
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null) break;
                Run(line);
            }
        }

        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }
    }
}
