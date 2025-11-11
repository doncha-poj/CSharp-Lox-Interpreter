using System.Collections.Generic;

namespace cslox
{
    /// <summary>
    /// An interface for all Lox objects that can be "called"
    /// (like functions and, later, class constructors).
    /// </summary>
    public interface ILoxCallable
    {
        /// <summary>
        /// Gets the number of arguments the callable expects.
        /// </summary>
        int Arity { get; }

        /// <summary>
        /// Executes the callable.
        /// </summary>
        object Call(Interpreter interpreter, List<object> arguments);
    }
}