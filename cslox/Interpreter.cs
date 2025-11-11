using System;
using System.Collections.Generic;

namespace cslox
{
    public class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
    {
        private readonly Environment _globals = new Environment();
        private Environment _environment;

        /// <summary>
        /// A native Lox function that returns the current system time in seconds.
        /// This is a private class nested inside the Interpreter.
        /// </summary>
        private class NativeClock : ILoxCallable
        {
            public int Arity => 0;

            public object Call(Interpreter interpreter, List<object> arguments)
            {
                return (double)DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            }

            public override string ToString()
            {
                return "<native fn>";
            }
        }

        public Interpreter()
        {
            _environment = _globals;
            _globals.Define("clock", new NativeClock());
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
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

        // --- HELPER METHODS ---

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            Environment previous = this._environment;
            try
            {
                this._environment = environment;
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this._environment = previous;
            }
        }

        // --- STATEMENT VISITOR METHODS (UPDATED) ---

        public object VisitBlockStmt(BlockStmt stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(_environment));
            return null;
        }

        public object VisitExpressionStmt(ExpressionStmt stmt)
        {
            Evaluate(stmt.Expression);
            return null;
        }

        // --- NEW: IF STATEMENT ---
        public object VisitIfStmt(IfStmt stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                Execute(stmt.ElseBranch);
            }
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

        // --- NEW: WHILE STATEMENT ---
        public object VisitWhileStmt(WhileStmt stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                Execute(stmt.Body);
            }
            return null;
        }

        // --- EXPRESSION VISITOR METHODS (UPDATED) ---

        public object VisitAssignExpr(Assign expr)
        {
            object value = Evaluate(expr.Value);
            _environment.Assign(expr.Name, value);
            return value;
        }

        public object VisitVariableExpr(Variable expr)
        {
            return _environment.Get(expr.Name);
        }

        public object VisitLiteralExpr(Literal expr)
        {
            return expr.Value;
        }

        public object VisitGroupingExpr(Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLogicalExpr(Logical expr)
        {
            object left = Evaluate(expr.Left);

            if (expr.Operator.Type == TokenType.OR)
            {
                // Short-circuit: if left is truthy, return it
                if (IsTruthy(left)) return left;
            }
            else // Must be AND
            {
                // Short-circuit: if left is falsey, return it
                if (!IsTruthy(left)) return left;
            }

            // Only evaluate right if not short-circuited
            return Evaluate(expr.Right);
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
            return null; // Unreachable
        }

        public object VisitBinaryExpr(Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                // ... (MINUS, SLASH, STAR, PLUS, GREATER, etc. are all unchanged) ...
                case TokenType.MINUS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.SLASH:
                    CheckNumberOperands(expr.Operator, left, right);
                    if ((double)right == 0)
                        throw new RuntimeError(expr.Operator, "Division by zero.");
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
                case TokenType.PLUS:
                    if (left is double && right is double)
                        return (double)left + (double)right;
                    if (left is string && right is string)
                        return (string)left + (string)right;
                    throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings.");
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
                case TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
            }
            return null; // Unreachable
        }

        public object VisitCallExpr(Call expr)
        {
            object callee = Evaluate(expr.Callee);

            // Evaluate all the arguments
            var arguments = new List<object>();
            foreach (var argument in expr.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            // Check if the callee is actually a callable function
            if (callee is not ILoxCallable function)
            {
                throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
            }

            // Check if the argument count matches
            if (arguments.Count != function.Arity)
            {
                throw new RuntimeError(expr.Paren, $"Expected {function.Arity} arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }

        public object VisitFunctionStmt(FunctionStmt stmt)
        {
            // Create a new LoxFunction, capturing the *current* environment
            var function = new LoxFunction(stmt, _environment);

            // Define the function in the *current* environment
            _environment.Define(stmt.Name.Lexeme, function);
            return null;
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            return a.Equals(b);
        }

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

        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(op, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token op, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(op, "Operands must be numbers.");
        }

        public object VisitReturnStmt(ReturnStmt stmt)
        {
            object value = null;
            if (stmt.Value != null)
            {
                value = Evaluate(stmt.Value);
            }

            // Throw the special exception to unwind the stack
            throw new Return(value);
        }
    }
}