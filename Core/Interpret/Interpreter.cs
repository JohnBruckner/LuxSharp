using Core.Syntax;

namespace Core.Interpret;

public class Interpreter(Action<int, string> errorHandler) : IExprVisitor<object?>, IStmtVisitor<object?>
{
    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError e)
        {
            errorHandler(e.Token.Line, e.Message);
        }
    }

    private void Execute(Stmt statement)
    {
        statement.Accept(this);
    }

    public object VisitAssignExpr(Assign expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitBinaryExpr(Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case TokenType.MINUS:
            case TokenType.SLASH:
            case TokenType.STAR:
            case TokenType.GREATER:
            case TokenType.GREATER_EQUAL:
            case TokenType.LESS:
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.Op, left, right);
                break;
            case TokenType.PLUS:
                if (!((left is double && right is double) || (left is string && right is string)))
                {
                    throw new RuntimeError(expr.Op,
                        "Operands must be two numbers or two strings.");
                }

                break;
        }

        return expr.Op.Type switch
        {
            TokenType.GREATER => (double)left! > (double)right!,
            TokenType.GREATER_EQUAL => (double)left! >= (double)right!,
            TokenType.LESS => (double)left! < (double)right!,
            TokenType.LESS_EQUAL => (double)left! <= (double)right!,
            TokenType.BANG_EQUAL => !IsEqual(left, right),
            TokenType.EQUAL_EQUAL => IsEqual(left, right),
            TokenType.MINUS => (double)left! - (double)right!,
            TokenType.SLASH when (double)right! == 0 => throw new RuntimeError(expr.Op, "Cannot divide by zero"),
            TokenType.SLASH => (double)left! / (double)right!,
            TokenType.STAR => (double)left! * (double)right!,
            TokenType.PLUS => 
                left is double l && right is double r
                ? l + r
                : 
                left is string ls && right is string rs
                    ? ls + rs
                    : null,
            _ => null
        };
    }

    public object VisitCallExpr(Call expr)
    {
        throw new NotImplementedException();
    }

    public object VisitGetExpr(Get expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitGroupingExpr(Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object? VisitLiteralExpr(Literal expr)
    {
        return expr.Value;
    }

    public object VisitLogicalExpr(Logical expr)
    {
        throw new NotImplementedException();
    }

    public object VisitSetExpr(Set expr)
    {
        throw new NotImplementedException();
    }

    public object VisitSuperExpr(Super expr)
    {
        throw new NotImplementedException();
    }

    public object VisitThisExpr(This expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitUnaryExpr(Unary expr)
    {
        var right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case TokenType.MINUS:
                CheckNumberOperand(expr.Op, expr.Right);
                break;
        }

        return expr.Op.Type switch
        {
            TokenType.BANG => !IsTruthy(right),
            TokenType.MINUS => -(double)right!,
            _ => null
        };
    }

    public object? VisitVariableExpr(Variable expr)
    {
        throw new NotImplementedException();
    }
    
    public object? VisitExpressionStmt(Expression stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object? VisitPrintStmt(Print stmt)
    {
        var val = Evaluate(stmt.Expr);
        Console.WriteLine(val.Stringify());
        return null;
    }

    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    private bool IsTruthy(object? value)
    {
        return value switch
        {
            null => false,
            bool b => b,
            _ => true
        };
    }

    private bool IsEqual(object? a, object? b)
    {
        return a switch
        {
            null when b is null => true,
            null => false,
            _ => a.Equals(b)
        };
    }

    private void CheckNumberOperand(Token op, object operand)
    {
        if (operand is double) return;
        throw new RuntimeError(op, "Operand must be a number");
    }

    private void CheckNumberOperands(Token op, object? left, object? right)
    {
        if (left is double && right is double) return;
        throw new RuntimeError(op, "Operands must be numbers");
    }

    private class RuntimeError(Token token, string message) : Exception(message)
    {
        public Token Token { get; } = token;
    }
}