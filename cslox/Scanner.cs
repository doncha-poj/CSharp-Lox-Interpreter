using System.Collections.Generic;

namespace cslox
{
    public class Scanner
    {
        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();

        // Fields to track our position
        private int _start = 0;   // First char in the token we're scanning
        private int _current = 0; // Char we're currently looking at
        private int _line = 1;    // The current line number

        public Scanner(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are at the beginning of the next token.
                _start = _current;
                ScanToken(); // This is the main method we'll build
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, _line));
            return _tokens;
        }

        // Helper method to see if we've run out of code
        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        // This method consumes one character and returns it
        private char Advance()
        {
            _current++;
            return _source[_current - 1];
        }

        // This method creates a new token from the text we've consumed
        private void AddToken(TokenType type)
        {
            AddToken(type, null); // Overload for tokens without a literal value
        }

        private void AddToken(TokenType type, object? literal)
        {
            string text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, text, literal, _line));
        }

        // This is the method we need to build next.
        // It's empty for now, which will cause an infinite loop.
        private void ScanToken()
        {
            char c = Advance(); // Get the next character
            switch (c)
            {
                // Handle all the single-character tokens
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;

                // --- We will add more cases here ---

                // Handle whitespace
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;

                // Handle newlines
                case '\n':
                    _line++; // Increment line counter
                    break;

                // Handle unrecognized characters
                default:
                    Lox.Error(_line, "Unexpected character.");
                    break;
            }
        }
    }
}