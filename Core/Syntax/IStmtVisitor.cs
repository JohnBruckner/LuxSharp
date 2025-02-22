namespace Core.Syntax;

public interface IStmtVisitor<T>
{
    T VisitExpressionStmt(Expression stmt);
    T VisitPrintStmt(Print stmt);
    T VisitVarStmt(Var stmt);

}