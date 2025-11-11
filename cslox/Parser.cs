using System;
using System.Collections.Generic;

namespace cslox
{
    public class Parser
    {
        // Nested ParseError class
        private class ParseError : System.Exception { }

        private readonly List<Token> _tokens;
        private int _current = 0;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        /// <summary>
        /// The new main entry point. Parses a list of statements.
        /// </summary>
        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }
            return statements;
        }

        // --- GRAMMAR RULE METHODS ---
        // We start from the top-level "declaration" now.

        // declaration → varDecl | statement
        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.VAR)) return VarDeclaration();
                return Statement();
            }
            catch (ParseError)
            {
                Synchronize();
                return null; // Return null, we'll filter it out
            }
        }

        // varDecl → "var" IDENTIFIER ( "=" expression )? ";"
        private Stmt VarDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (Match(TokenType.EQUAL))
            {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new VarStmt(name, initializer);
        }

        // statement → exprStmt | printStmt
        private Stmt Statement()
        {
            if (Match(TokenType.PRINT)) return PrintStatement();

            return ExpressionStatement();
        }

        // printStmt → "print" expression ";"
        private Stmt PrintStatement()
        {
            Expr value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new PrintStmt(value);
        }

        // exprStmt → expression ";"
        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new ExpressionStmt(expr);
        }

        // expression → assignment
        private Expr Expression()
        {
            return Assignment();
        }

        // assignment → IDENTIFIER "=" assignment | equality
        private Expr Assignment()
        {
            Expr expr = Equality();

            if (Match(TokenType.EQUAL))
            {
                Token equals = Previous();
                Expr value = Assignment(); // Right-associative

                if (expr is Variable variableExpr)
                {
                    Token name = variableExpr.Name;
                    return new Assign(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        // equality → comparison ( ( "!=" | "==" ) comparison )*
        // (Unchanged)
        private Expr Equality()
        {
            Expr expr = Comparison();
            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token op = Previous();
                Expr right = Comparison();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        // comparison → term ( ( ">" | ">=" | "<" | "<=" ) term )*
        // (Unchanged)
        private Expr Comparison()
        {
            Expr expr = Term();
            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token op = Previous();
                Expr right = Term();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        // term → factor ( ( "-" | "+" ) factor )*
        // (Unchanged)
        private Expr Term()
        {
            Expr expr = Factor();
            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token op = Previous();
                Expr right = Factor();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        // factor → unary ( ( "/" | "*" ) unary )*
        // (Unchanged)
        private Expr Factor()
        {
            Expr expr = Unary();
            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                Token op = Previous();
                Expr right = Unary();
                expr = new Binary(expr, op, right);
            }
            return expr;
        }

        // unary → ( "!" | "-" ) unary | primary
        // (Unchanged)
        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token op = Previous();
                Expr right = Unary();
                return new Unary(op, right);
            }
            return Primary();
        }

        // primary → ... | IDENTIFIER
        // (Updated to add IDENTIFIER)
        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.NIL)) return new Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Literal(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER)) // <-- ADDED
            {
                return new Variable(Previous());
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }


        // --- HELPER & ERROR HANDLING METHODS ---

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

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return _tokens[_current];
        }

        private Token Previous()
        {
            return _tokens[_current - 1];
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        private void Synchronize()
        {
            Advance();
            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.SEMICOLON) return;
                switch (Peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }
                Advance();
            }
        }
    }
}