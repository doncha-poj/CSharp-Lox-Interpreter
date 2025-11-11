using System.Collections.Generic;
using System.Data.Common;

namespace cslox
{
    public class Scanner
    {
        private static readonly Dictionary<string, TokenType> Keywords;

        static Scanner() // This is a static constructor
        {
            Keywords = new Dictionary<string, TokenType>
    {
        {"and",    TokenType.AND},
        {"class",  TokenType.CLASS},
        {"else",   TokenType.ELSE},
        {"false",  TokenType.FALSE},
        {"for",    TokenType.FOR},
        {"fun",    TokenType.FUN},
        {"if",     TokenType.IF},
        {"nil",    TokenType.NIL},
        {"or",     TokenType.OR},
        {"print",  TokenType.PRINT},
        {"return", TokenType.RETURN},
        {"super",  TokenType.SUPER},
        {"this",   TokenType.THIS},
        {"true",   TokenType.TRUE},
        {"var",    TokenType.VAR},
        {"while",  TokenType.WHILE}
    };
        }

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

                // Handle '!' and '!='
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                // Handle '=' and '=='
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                // Handle '<' and '<='
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                // Handle '>' and '>='
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                // Handle '/' (SLASH) or '//' (COMMENT)
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line.
                        // We use Peek() to look ahead without consuming the newline
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;

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
                // Handle string literals
                case '"':
                    String(); // Call our new string helper
                    break;

                // Handle unrecognized characters
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(_line, "Unexpected character.");
                    }
                    break;
            }
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            // Get the text of the identifier
            string text = _source.Substring(_start, _current - _start);

            // See if it's a keyword
            if (!Keywords.TryGetValue(text, out TokenType type))
            {
                // Not a keyword, it's a user-defined identifier
                type = TokenType.IDENTIFIER;
            }

            AddToken(type);
        }

        private void Number()
        {
            // Consume all consecutive digits
            while (IsDigit(Peek())) Advance();

            // Look for a fractional part
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the "."
                Advance();

                // Consume the remaining digits
                while (IsDigit(Peek())) Advance();
            }

            // Parse the string into a real C# double
            // uses InvariantCulture to ensure '.' is always treated as a decimal point,
            // regardless of the computer's regional settings.
            string numberText = _source.Substring(_start, _current - _start);
            double value = double.Parse(numberText, System.Globalization.CultureInfo.InvariantCulture);

            AddToken(TokenType.NUMBER, value);
        }

        private void String()
        {
            // Keep consuming characters until we hit the closing "
            while (Peek() != '"' && !IsAtEnd())
            {
                // Lox supports multi-line strings
                if (Peek() == '\n') _line++;
                Advance();
            }

            // Check for an unterminated string
            if (IsAtEnd())
            {
                Lox.Error(_line, "Unterminated string.");
                return;
            }

            // Consume the closing "
            Advance();

            // Extract the string value (trimming the surrounding quotes)
            string value = _source.Substring(_start + 1, _current - _start - 2);
            AddToken(TokenType.STRING, value);
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (_source[_current] != expected) return false;

            _current++; // Consume the character
            return true;
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0'; // '\0' is the C# "null" character
            return _source[_current];
        }

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length) return '\0';
            return _source[_current + 1];
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
    }
}