namespace cslox
{
    public interface IExprVisitor<T>
    {
        T VisitBinaryExpr(Binary expr);
        T VisitGroupingExpr(Grouping expr);
        T VisitLiteralExpr(Literal expr);
        T VisitUnaryExpr(Unary expr);
        T VisitVariableExpr(Variable expr);
        T VisitAssignExpr(Assign expr);
        T VisitLogicalExpr(Logical expr);
        T VisitCallExpr(Call expr);
    }

    public abstract class Expr
    {
        // This is the core of the Visitor Pattern.
        // It forces every expression class to implement this method,
        // which "accepts" a visitor.
        public abstract T Accept<T>(IExprVisitor<T> visitor);
    }

    /// <summary>
    /// Represents a binary operation (e.g., 1 + 2)
    /// </summary>
    public class Binary : Expr
    {
        public readonly Expr Left;
        public readonly Token Operator;
        public readonly Expr Right;

        public Binary(Expr left, Token op, Expr right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            // This is the "double dispatch":
            // 1. We call Accept() on the Binary object.
            // 2. It calls VisitBinaryExpr() on the visitor,
            //    passing *itself* (this) as the argument.
            return visitor.VisitBinaryExpr(this);
        }
    }

    /// <summary>
    /// Represents a grouping (e.g., (1 + 2))
    /// </summary>
    public class Grouping : Expr
    {
        public readonly Expr Expression;

        public Grouping(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

    /// <summary>
    /// Represents a literal value (e.g., 123, "hello", true, nil)
    /// </summary>
    public class Literal : Expr
    {
        public readonly object? Value; // The value (123, "hello", etc.)

        public Literal(object? value)
        {
            Value = value;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    /// <summary>
    /// Represents a unary operation (e.g., -1, !true)
    /// </summary>
    public class Unary : Expr
    {
        public readonly Token Operator;
        public readonly Expr Right;

        public Unary(Token op, Expr right)
        {
            Operator = op;
            Right = right;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }

    /// <summary>
    /// Represents a variable expression in the abstract syntax tree (AST).
    /// </summary>
    /// <remarks>A variable expression is used to reference a variable by its name within the context of an
    /// expression. This class is part of the visitor pattern implementation for processing expressions.</remarks>
    public class Variable : Expr
    {
        public readonly Token Name;
        public Variable(Token name)
        {
            Name = name;
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }
    }

    /// <summary>
    /// Represents an assignment expression, which associates a value with a variable or property.
    /// </summary>
    /// <remarks>This expression is used to assign a value to a variable or property within the context of an
    /// expression tree. The <see cref="Name"/> property identifies the variable or property being assigned, and the
    /// <see cref="Value"/>  property represents the expression whose result will be assigned.</remarks>
    public class Assign : Expr
    {
        public readonly Token Name;
        public readonly Expr Value;
        public Assign(Token name, Expr value)
        {
            Name = name;
            Value = value;
        }
        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitAssignExpr(this);
        }
    }

    /// <summary>
    /// Represents a logical 'and' or 'or' operation.
    /// </summary>
    public class Logical : Expr
    {
        public readonly Expr Left;
        public readonly Token Operator;
        public readonly Expr Right;

        public Logical(Expr left, Token op, Expr right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitLogicalExpr(this);
        }
    }

    /// <summary>
    /// Represents a function or method call expression in the abstract syntax tree (AST).
    /// </summary>
    /// <remarks>A <see cref="Call"/> expression consists of the callee (the entity being called),  the
    /// arguments passed to the call, and the closing parenthesis token for error reporting. This class is typically
    /// used in the context of parsing or interpreting code.</remarks>
    public class Call : Expr
    {
        public readonly Expr Callee;
        public readonly Token Paren;
        public readonly List<Expr> Arguments;

        public Call(Expr callee, Token paren, List<Expr> arguments)
        {
            Callee = callee;
            Paren = paren;
            Arguments = arguments;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitCallExpr(this);
        }
    }
}