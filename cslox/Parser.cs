using System.Collections.Generic;

namespace cslox
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current = 0;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        // We will add the main Parse() method here later.

        // --- HELPER METHODS ---

        /// <summary>
        /// Checks if the current token is one of the given types.
        /// If so, it consumes the token and returns true.
        /// </summary>
        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the current token is of the given type
        /// (without consuming it).
        /// </summary>
        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        /// <summary>
        /// Consumes the current token and returns it.
        /// </summary>
        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        /// <summary>
        /// Checks if we've run out of tokens.
        /// </summary>
        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        /// <summary>
        /// Returns the current token without consuming it.
        /// </summary>
        private Token Peek()
        {
            return _tokens[_current];
        }

        /// <summary>
        /// Returns the previous token.
        /// </summary>
        private Token Previous()
        {
            return _tokens[_current - 1];
        }
    }
}