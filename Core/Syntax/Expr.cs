namespace Core.Syntax;

public abstract class Expr
{
    public abstract T Accept<T>(IExprVisitor<T> exprVisitor);
}

public class Assign(Token name, Expr value) : Expr
{
    public readonly Token Name = name;
    public readonly Expr Value = value;

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitAssignExpr(this);
    }
}

public class Binary(Expr left, Token op, Expr right) : Expr
{
    public readonly Expr Left = left;
    public readonly Expr Right = right;
    public readonly Token Op = op;

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitBinaryExpr(this);
    }
}

public class Call(Expr calle, Token paren, List<Expr> arguments) : Expr
{
    public readonly Expr Calee = calle;
    public readonly Token Paren = paren;
    public readonly List<Expr> Arguments = arguments;
    
    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitCallExpr(this);
    }
}

public class Get(Expr obj, Token name) : Expr
{
    public readonly Expr Object = obj;
    public readonly Token Name = name;

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitGetExpr(this);
    }
}

public class Grouping(Expr expression) : Expr
{
    public readonly Expr Expression = expression;

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitGroupingExpr(this);
    }
}

public class Literal(object? value) : Expr
{
    public readonly object? Value = value;

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitLiteralExpr(this);
    }
}

public class Logical(Expr left, Token op, Expr right) : Expr
{
    public readonly Expr Left = left;
    public readonly Token Op = op;
    public readonly Expr Right = right;

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitLogicalExpr(this);
    }
}

public class Set(Expr obj, Token name, Expr value) : Expr
{
    public readonly Expr Object = obj;
    public readonly Token Name = name;
    public readonly Expr Value = value;
    
    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitSetExpr(this);
    }
}

public class Super(Token keyword, Token method) : Expr
{
    public readonly Token Keyword = keyword;
    public readonly Token Method = method;

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitSuperExpr(this);
    }
}

public class This(Token keyword) : Expr
{
    public readonly Token Keyword = keyword;

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitThisExpr(this);
    }
}

public class Unary(Token op, Expr right) : Expr
{
    public readonly Token Op = op;
    public readonly Expr Right = right;

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitUnaryExpr(this);
    }
}

public class Variable(Token name) : Expr
{
    public readonly Token Name = name;

    public override T Accept<T>(IExprVisitor<T> exprVisitor)
    {
        return exprVisitor.VisitVariableExpr(this);
    }
}