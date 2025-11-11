using System;
using System.Collections.Generic;

namespace cslox
{
    public class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
    {
        // the memory
        private Environment _environment = new Environment();
        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
                    // Filter out null statements from parser errors
                    if (statement != null)
                    {
                        Execute(statement);
                    }
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        public object VisitIfStmt(IfStmt stmt)
        {
            // 1. Evaluate the condition
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                // 2. If true, execute the 'then' branch
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                // 3. If false and there is an 'else' branch, execute it
                Execute(stmt.ElseBranch);
            }

            return null; // Statements don't return values
        }

        public object VisitWhileStmt(WhileStmt stmt)
        {
            // 1. Loop as long as the condition is truthy
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                // 2. Execute the body
                Execute(stmt.Body);
            }

            return null;
        }

        public object VisitExpressionStmt(ExpressionStmt stmt)
        {
            Evaluate(stmt.Expression); // Evaluate for side-effects
            return null;
        }

        public object VisitPrintStmt(PrintStmt stmt)
        {
            object value = Evaluate(stmt.Expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object VisitVarStmt(VarStmt stmt)
        {
            object value = null;
            if (stmt.Initializer != null)
            {
                value = Evaluate(stmt.Initializer);
            }

            _environment.Define(stmt.Name.Lexeme, value);
            return null;
        }

        public object VisitLiteralExpr(Literal expr)
        {
            return expr.Value;
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitUnaryExpr(Unary expr)
        {
            object right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    CheckNumberOperand(expr.Operator, right);
                    return -(double)right;
            }

            // Unreachable.
            return null;
        }

        public object VisitBinaryExpr(Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                // Arithmetic
                case TokenType.MINUS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.SLASH:
                    CheckNumberOperands(expr.Operator, left, right);
                    if ((double)right == 0)
                    {
                        throw new RuntimeError(expr.Operator, "Division by zero.");
                    }
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
                case TokenType.PLUS:
                    // Special case: + can add numbers or concatenate strings
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }
                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }
                    // If types are mixed, throw an error
                    throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings.");

                // Comparison
                case TokenType.GREATER:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;

                // Equality
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
            }

            // Unreachable.
            return null;
        }

        public object VisitVariableExpr(Variable expr)
        {
            return _environment.Get(expr.Name);
        }

        public object VisitAssignExpr(Assign expr)
        {
            object value = Evaluate(expr.Value);
            _environment.Assign(expr.Name, value);
            return value; // Assignment is an expression
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public object VisitBlockStmt(BlockStmt stmt)
        {
            // Create a *new* environment for this block,
            // with the current environment as its parent.
            ExecuteBlock(stmt.Statements, new Environment(_environment));
            return null;
        }

        public void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            // 1. Get the current environment before the block
            Environment previous = this._environment;

            try
            {
                // 2. Set the *new* environment for the block
                this._environment = environment;

                // 3. Execute all statements *inside* that new environment
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                // 4. IMPORTANT: Restore the previous environment
                //    This 'finally' block ensures we exit the scope
                //    even if an error (exception) occurs.
                this._environment = previous;
            }
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        /// <summary>
        /// Checks if an operand is a number. Throws RuntimeError if not.
        /// </summary>
        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(op, "Operand must be a number.");
        }

        /// <summary>
        /// Checks if both operands are numbers. Throws RuntimeError if not.
        /// </summary>
        private void CheckNumberOperands(Token op, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(op, "Operands must be numbers.");
        }

        /// <summary>
        /// Lox's definition of "truth": false and nil are falsey,
        /// everything else is truthy.
        /// </summary>
        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true; // All other values (numbers, strings) are true
        }

        /// <summary>
        /// Lox's definition of equality.
        /// </summary>
        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            return a.Equals(b);
        }

        /// <summary>
        /// Converts a Lox value to a string for display.
        /// </summary>
        private string Stringify(object obj)
        {
            if (obj == null) return "nil";

            if (obj is double)
            {
                string text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return obj.ToString();
        }
    }
}