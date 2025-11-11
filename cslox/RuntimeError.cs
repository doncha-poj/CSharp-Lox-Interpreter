namespace cslox
{
    /// <summary>
    /// Represents an error that occurs during the runtime execution of a program.
    /// </summary>
    /// <remarks>This exception is typically used to signal errors that occur during the interpretation or
    /// execution of code, such as invalid operations or unexpected runtime conditions. The <see cref="Token"/> property
    /// provides additional context about the location in the source code where the error occurred.</remarks>
    public class RuntimeError : System.Exception
    {
        public readonly Token Token;

        public RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }
    }
}