using System.Collections.Generic;

namespace cslox
{
    // -----------------------------------------------------------------
    // 1. The Visitor Interface
    // -----------------------------------------------------------------
    public interface IStmtVisitor<T>
    {
        T VisitBlockStmt(BlockStmt stmt);
        T VisitExpressionStmt(ExpressionStmt stmt);
        T VisitIfStmt(IfStmt stmt);
        T VisitPrintStmt(PrintStmt stmt);
        T VisitVarStmt(VarStmt stmt);
        T VisitWhileStmt(WhileStmt stmt);
        T VisitFunctionStmt(FunctionStmt stmt);
        T VisitReturnStmt(ReturnStmt stmt);
    }

    // -----------------------------------------------------------------
    // 2. The Abstract Base Class
    // -----------------------------------------------------------------
    public abstract class Stmt
    {
        public abstract T Accept<T>(IStmtVisitor<T> visitor);
    }

    // -----------------------------------------------------------------
    // 3. The Statement Subclasses
    // -----------------------------------------------------------------

    /// <summary>
    /// A new statement type for a code block.
    /// </summary>
    public class BlockStmt : Stmt
    {
        public readonly List<Stmt> Statements;

        public BlockStmt(List<Stmt> statements)
        {
            Statements = statements;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }

    /// <summary>
    /// A statement that is just an expression (e.g., "1 + 2;")
    /// (Unchanged)
    /// </summary>
    public class ExpressionStmt : Stmt
    {
        public readonly Expr Expression;

        public ExpressionStmt(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }

    /// <summary>
    /// A new statement type for an if-then-else.
    /// </summary>
    public class IfStmt : Stmt
    {
        public readonly Expr Condition;
        public readonly Stmt ThenBranch;
        public readonly Stmt ElseBranch; // Can be null

        public IfStmt(Expr condition, Stmt thenBranch, Stmt elseBranch)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitIfStmt(this);
        }
    }

    /// <summary>
    /// A print statement (e.g., "print 1 + 2;")
    /// (Unchanged)
    /// </summary>
    public class PrintStmt : Stmt
    {
        public readonly Expr Expression;

        public PrintStmt(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }

    /// <summary>
    /// A variable declaration (e.g., "var a = 1;")
    /// (Unchanged)
    /// </summary>
    public class VarStmt : Stmt
    {
        public readonly Token Name;
        public readonly Expr Initializer;

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

    /// <summary>
    /// A new statement type for a while loop.
    /// </summary>
    public class WhileStmt : Stmt
    {
        public readonly Expr Condition;
        public readonly Stmt Body;

        public WhileStmt(Expr condition, Stmt body)
        {
            Condition = condition;
            Body = body;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }
    }

    public class FunctionStmt : Stmt
    {
        public readonly Token Name;
        public readonly List<Token> Parameters;
        public readonly List<Stmt> Body;

        public FunctionStmt(Token name, List<Token> parameters, List<Stmt> body)
        {
            Name = name;
            Parameters = parameters;
            Body = body;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitFunctionStmt(this);
        }
    }

    public class ReturnStmt : Stmt
    {
        public readonly Token Keyword;
        public readonly Expr Value; // Can be null

        public ReturnStmt(Token keyword, Expr value)
        {
            Keyword = keyword;
            Value = value;
        }

        public override T Accept<T>(IStmtVisitor<T> visitor)
        {
            return visitor.VisitReturnStmt(this);
        }
    }
}