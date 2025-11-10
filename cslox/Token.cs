namespace cslox
{
    public class Token
    {
        public readonly TokenType Type;
        public readonly string Lexeme;
        public readonly object? Literal; // ? means it can be null
        public readonly int Line;

        public Token(TokenType type, string lexeme, object? literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public override string ToString()
        {
            return $"{Type} {Lexeme} {Literal}";
        }
    }
}