namespace Core.Syntax;

public abstract class Stmt
{
    public abstract T Accept<T>(IStmtVisitor<T> stmtVisitor);
}

public class Expression(Expr expr) : Stmt
{
    public readonly Expr Expr = expr;

    public override T Accept<T>(IStmtVisitor<T> stmtVisitor)
    {
        return stmtVisitor.VisitExpressionStmt(this);
    }
}

public class Print(Expr expr) : Stmt
{
    public readonly Expr Expr = expr;

    public override T Accept<T>(IStmtVisitor<T> stmtVisitor)
    {
        return stmtVisitor.VisitPrintStmt(this);
    }
}


