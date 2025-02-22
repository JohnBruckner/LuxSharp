using System.Text;

namespace Core.Syntax
{
    public class AstPrinter : IExprVisitor<string>
    {
        public string VisitAssignExpr(Assign expr) => throw new NotImplementedException();
        public string VisitCallExpr(Call expr) => throw new NotImplementedException();
        public string VisitGetExpr(Get expr) => throw new NotImplementedException();
        public string VisitLogicalExpr(Logical expr) => throw new NotImplementedException();
        public string VisitSetExpr(Set expr) => throw new NotImplementedException();
        public string VisitSuperExpr(Super expr) => throw new NotImplementedException();
        public string VisitThisExpr(This expr) => throw new NotImplementedException();
        public string VisitVariableExpr(Variable expr) => throw new NotImplementedException();
        
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitBinaryExpr(Binary expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Literal expr)
        {
            if (expr.Value is null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Unary expr)
        {
            return Parenthesize(expr.Op.Lexeme, expr.Right);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append('(').Append(name);
            foreach (Expr expr in exprs)
            {
                builder.Append(' ');
                builder.Append(expr.Accept(this));
            }
            builder.Append(')');

            return builder.ToString();
        }
    }
}