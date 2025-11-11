using System;

namespace cslox
{
    /// <summary>
    /// A special exception used to unwind the stack for 'return' statements.
    /// </summary>
    public class Return : Exception
    {
        public readonly object Value;

        public Return(object value)
            : base(null, null) // Disable exception overhead
        {
            Value = value;
        }
    }
}