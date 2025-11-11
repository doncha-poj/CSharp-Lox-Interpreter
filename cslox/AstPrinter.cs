using System.Text;

namespace cslox
{
    // This class implements the visitor interface.
    // It returns a string.
    public class AstPrinter : IExprVisitor<string>
    {
        // Public entry point
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }


        public string VisitBinaryExpr(Binary expr)
        {
            // Recursively calls Parenthesize for left, op, and right
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        public string VisitVariableExpr(Variable expr)
        {
            return expr.Name.Lexeme;
        }

        public string VisitAssignExpr(Assign expr)
        {
            return Parenthesize($"= {expr.Name.Lexeme}", expr.Value);
        }

        public string VisitLogicalExpr(Logical expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitCallExpr(Call expr)
        {
            var builder = new StringBuilder();
            builder.Append("(");
            builder.Append(expr.Callee.Accept(this));
            foreach (var argument in expr.Arguments)
            {
                builder.Append(" ");
                builder.Append(argument.Accept(this));
            }
            builder.Append(")");
            return builder.ToString();
        }

        /// <summary>
        /// Wraps an expression and its sub-expressions in parentheses.
        /// </summary>
        private string Parenthesize(string name, params Expr[] exprs)
        {
            var builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (var expr in exprs)
            {
                builder.Append(" ");
                // This is the recursion: we call Accept on the sub-expression
                // which will, in turn, call one of our Visit methods.
                builder.Append(expr.Accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}