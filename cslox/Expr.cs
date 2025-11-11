namespace cslox
{
    // -----------------------------------------------------------------
    // 1. The Visitor Interface
    // This defines the "contract" for a visitor.
    // It must have a Visit method for EACH expression type.
    // The <T> makes it "generic," so it can return any type.
    // -----------------------------------------------------------------
    public interface IExprVisitor<T>
    {
        T VisitBinaryExpr(Binary expr);
        T VisitGroupingExpr(Grouping expr);
        T VisitLiteralExpr(Literal expr);
        T VisitUnaryExpr(Unary expr);
    }

    // -----------------------------------------------------------------
    // 2. The Abstract Base Class
    // All expression classes (Binary, Literal, etc.)
    // must inherit from this.
    // -----------------------------------------------------------------
    public abstract class Expr
    {
        // This is the core of the Visitor Pattern.
        // It forces every expression class to implement this method,
        // which "accepts" a visitor.
        public abstract T Accept<T>(IExprVisitor<T> visitor);
    }

    // -----------------------------------------------------------------
    // 3. The Expression Subclasses
    // Each of these represents one rule in our grammar.
    // -----------------------------------------------------------------

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
}