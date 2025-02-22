using Core.Syntax;

namespace Core.Parse;

public class Parser(List<Token> tokens, Action<Token, string> errorHandler)
{
    private int _current = 0;


    public List<Stmt?> Parse()
    {
        var statements = new List<Stmt?>();
        while (!IsAtEnd())
        {
            statements.Add(Statement());
        }

        return statements;
    }

    private Stmt Statement()
    {
        if (Match(TokenType.PRINT))
            return PrintStatement();
        return ExpressionStatement();
    }

    private Stmt ExpressionStatement()
    {
        Expr expr = ExpressionParser();
        Consume(TokenType.SEMICOLON, "Expect ';' after expression");
        return new Expression(expr);
    }

    private Stmt PrintStatement()
    {
        Expr value = ExpressionParser();
        Consume(TokenType.SEMICOLON, "Expect ';' after value");
        return new Print(value);
    }

    private Expr ExpressionParser()
    {
        return Equality();
    }

    private Expr Equality()
    {
        Expr expr = Compare();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            Token op = Previous();
            Expr right = Compare();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Compare()
    {
        Expr expr = Term();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token op = Previous();
            Expr right = Term();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Term()
    {
        Expr expr = Factor();

        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            Token op = Previous();
            Expr right = Factor();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Factor()
    {
        Expr expr = Unary();

        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            Token op = Previous();
            Expr right = Unary();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            Token op = Previous();
            Expr right = Unary();
            return new Unary(op, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.FALSE))
        {
            return new Literal(false);
        }

        if (Match(TokenType.TRUE))
        {
            return new Literal(true);
        }

        if (Match(TokenType.NIL))
        {
            return new Literal(null);
        }

        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Literal(Previous().Literal);
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            Expr expr = ExpressionParser();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression");
            return new Grouping(expr);
        }

        throw Error(Peek(), "Expected expression");
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check((type)))
        {
            return Advance();
        }

        throw Error(Peek(), message);
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON) return;

            switch (Peek().Type)
            {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd())
        {
            return false;
        }

        return Peek().Type == type;
    }


    private Token Advance()
    {
        if (!IsAtEnd())
        {
            _current++;
        }

        return Previous();
    }


    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek()
    {
        return tokens[_current];
    }

    private Token Previous()
    {
        return tokens[_current - 1];
    }

    private ParseError Error(Token token, string message)
    {
        errorHandler(token, message);
        return new ParseError(message);
    }

    private class ParseError(string message) : Exception(message);
}