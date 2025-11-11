using System;

namespace cslox
{
    public class Interpreter : IExprVisitor<object>
    {
        // 1. Nested exception class
        private class RuntimeError : System.Exception
        {
            public readonly Token Token;

            public RuntimeError(Token token, string message) : base(message)
            {
                Token = token;
            }
        }

        // 2. Main entry point
        public void Interpret(Expr expression)
        {
            try
            {
                object value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        // 3. Helper to evaluate an expression
        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        // 4. Helper to display the value
        private string Stringify(object obj)
        _s{
            if (obj == null) return "nil";
            
            // C# adds .0 to integer-valued doubles,
            // so we check and remove it.
            if (obj is double)
            {
                string text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

return obj.ToString();
        }

        // --- WE WILL ADD VISIT METHODS HERE ---
    }
}
