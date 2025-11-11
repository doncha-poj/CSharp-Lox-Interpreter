using System.Collections.Generic;

namespace cslox
{
    public class Environment
    {
        public readonly Environment Enclosing;
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public Environment()
        {
            Enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            Enclosing = enclosing;
        }

        /// <summary>
        /// Defines a new variable in the current scope.
        /// </summary>
        public void Define(string name, object value)
        {
            // We use [] for assignment, which allows
            // 'var a = 1; var a = 2;'
            // This is simple, but we'll improve it later.
            _values[name] = value;
        }

        /// <summary>
        /// Gets the value of a variable.
        /// Throws a RuntimeError if the variable is not defined.
        /// </summary>
        public object Get(Token name)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                return _values[name.Lexeme];
            }

            if (Enclosing != null)
            {
                return Enclosing.Get(name);
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }

        /// <summary>
        /// Assigns a new value to an *existing* variable.
        /// Throws a RuntimeError if the variable is not defined.
        /// </summary>
        public void Assign(Token name, object value)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return;
            }

            if (Enclosing != null)
            {
                Enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}
