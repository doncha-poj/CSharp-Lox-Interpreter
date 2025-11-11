// Author: Donald Chanthirath

using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace cslox
{
    public static class Lox
    {
        private static readonly Interpreter _interpreter = new Interpreter();
        static bool hadError = false;
        static bool hadRuntimeError = false;
        private static readonly AstPrinter _astPrinter = new AstPrinter();

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
            if (hadRuntimeError) System.Environment.Exit(70);
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
                hadRuntimeError = false;
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

            Parser parser = new Parser(tokens);
            Expr expression = parser.Parse();

            // Stop if there was a syntax error.
            if (hadError) return;

            // Stop if there was a runtime error.
            if (hadRuntimeError) return;

            // If no error, print the AST
            //if (expression != null)
            //{
            //    Console.WriteLine(_astPrinter.Print(expression));
            //}

            _interpreter.Interpret(expression);
        }
        /// <summary>
        /// Reports an error that occurred on a specific line with a given message.
        /// </summary>
        /// <param name="line">The line number where the error occurred. Must be a non-negative integer.</param>
        /// <param name="message">A description of the error. Cannot be null or empty.</param>
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

        /// <summary>
        /// The main error reporting method
        /// </summary>
        /// <param name="line"></param>
        /// <param name="message"></param>
        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $" at '{token.Lexeme}'", message);
            }
        }
        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
            hadRuntimeError = true;
        }
    }
}
