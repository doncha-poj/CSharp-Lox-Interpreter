using System.Collections.Generic;

namespace cslox
{
    public class LoxFunction : ILoxCallable
    {
        private readonly FunctionStmt _declaration;
        private readonly Environment _closure; // The environment where the function was *defined*

        public LoxFunction(FunctionStmt declaration, Environment closure)
        {
            _declaration = declaration;
            _closure = closure;
        }

        public int Arity => _declaration.Parameters.Count;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(_closure);
            for (int i = 0; i < _declaration.Parameters.Count; i++)
            {
                environment.Define(_declaration.Parameters[i].Lexeme, arguments[i]);
            }

            // Wrap the execution in a try...catch
            try
            {
                interpreter.ExecuteBlock(_declaration.Body, environment);
            }
            catch (Return returnValue)
            {
                // If we catch a 'return', return its value.
                return returnValue.Value;
            }

            // Otherwise, it's an implicit 'return nil'
            return null;
        }

        public override string ToString()
        {
            return $"<fn {_declaration.Name.Lexeme}>";
        }
    }
}
