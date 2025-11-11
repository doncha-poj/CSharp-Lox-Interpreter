using System.Collections.Generic;

namespace cslox
{
    // 1. The Visitor Interface (for Statements)
    public interface IStmtVisitor<T>
    {
        T VisitExpressionStmt(ExpressionStmt stmt);
        T VisitPrintStmt(PrintStmt stmt);
        T VisitVarStmt(VarStmt stmt);
    }

    // 2. The Abstract Base Class
    public abstract class Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }

    // 3. The Statement Subclasses

    /// <summary>
    /// A statement that is just an expression (e.g., "1 + 2;")
    /// </summary>
    public class ExpressionStmt : Stmt
    {
        public readonly Expr Expression;

        public ExpressionStmt(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }

    /// <summary>
    /// A print statement (e.g., "print 1 + 2;")
    /// </summary>
    public class PrintStmt : Stmt
    {
        public readonly Expr Expression;

        public PrintStmt(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IExprVisitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

    /// <summary>
    /// A variable declaration (e.g., "var a = 1;")
    /// </summary>
    public class VarStmt : Stmt
    {
        public readonly Token Name;
        public readonly Expr Initializer; // Can be null

        public VarStmt(Token name, Expr initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitVarStmt(this);
        }
    }
}