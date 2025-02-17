namespace Core;

public class Scanner
{
    public readonly string _source;
    private readonly List<Token> _tokens = [];
    private readonly Action<int, string> _errorHandler;
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;
    private bool _isAtEnd => _current >= _source.Length;

    public Scanner(string source, Action<int, string> errorHandler)
    {
        _source = source;
        _errorHandler = errorHandler;
    }

    public List<Token> ScanTokens()
    {
        while (!_isAtEnd)
        {
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.EOF, "", null, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        var t = c switch
        {
            '(' => new TokenResult(TokenType.LEFT_PAREN, false),
            ')' => new TokenResult(TokenType.RIGHT_PAREN, false),
            '{' => new TokenResult(TokenType.LEFT_BRACE, false),
            '}' => new TokenResult(TokenType.RIGHT_BRACE, false),
            ',' => new TokenResult(TokenType.COMMA, false),
            '.' => new TokenResult(TokenType.DOT, false),
            '-' => new TokenResult(TokenType.MINUS, false),
            '+' => new TokenResult(TokenType.PLUS, false),
            ';' => new TokenResult(TokenType.SEMICOLON, false),
            '*' => new TokenResult(TokenType.STAR, false),
            '!' => Match('=')
                ? new TokenResult(TokenType.BANG_EQUAL, false)
                : new TokenResult(TokenType.BANG, false),
            '=' => Match('=')
                ? new TokenResult(TokenType.EQUAL_EQUAL, false)
                : new TokenResult(TokenType.EQUAL, false),
            '<' => Match('=')
                ? new TokenResult(TokenType.LESS_EQUAL, false)
                : new TokenResult(TokenType.LESS, false),
            '>' => Match('=')
                ? new TokenResult(TokenType.GREATER_EQUAL, false)
                : new TokenResult(TokenType.GREATER, false),
            '/' => HandleSlashToken(),
            ' ' => new TokenResult(TokenType.WHITESPACE, false),
            '\r' => new TokenResult(TokenType.WHITESPACE, false),
            '\t' => new TokenResult(TokenType.WHITESPACE, false),
            '\n' => HandleNewLine(),
            '"' => HandleString(),
            _ => IsDigit(c) ? 
                    HandleNumber() : 
                IsAlpha(c) ? 
                    HandleIdentifier() : 
                new TokenResult(TokenType.ERROR, true)    
        };

        if (t.IsError)
        {
            _errorHandler(_line, c.ToString());
        }
        else if (t.Type is not TokenType.WHITESPACE)
        {
            AddToken(t.Type, t.Literal);
        }
    }

    private bool Match(char expected)
    {
        if (_isAtEnd) return false;
        if (_source[_current] != expected) return false;

        _current++;
        return true;
    }

    private char Advance()
    {
        _current++;
        return _source[_current - 1];
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        var text = _source[_start.._current];
        _tokens.Add(new Token(type, text, literal, _line));
    }

    private char Peek()
    {
        return _isAtEnd ? '\0' : _source[_current];
    }

    private char PeekNext()
    {
        return _current + 1 >= _source.Length ? '\0' : _source[_current + 1];
    }

    private TokenResult HandleSlashToken()
    {
        if (!Match('/')) return new TokenResult(TokenType.SLASH, false);
        while (Peek() != '\n' && !_isAtEnd) Advance();
        return new TokenResult(TokenType.WHITESPACE, false);
    }

    private TokenResult HandleNewLine()
    {
        _line++;
        return new TokenResult(TokenType.WHITESPACE, false);
    }

    private TokenResult HandleString()
    {
        while (Peek() != '"' && !_isAtEnd)
        {
            if (Peek() == '\n')
            {
                _line++;
            }

            Advance();
        }

        if (_isAtEnd)
        {
            return new TokenResult(TokenType.ERROR, true);
        }

        Advance();
        string value = _source[(_start + 1)..(_current - 1)];
        return new TokenResult(TokenType.STRING, false, value);
    }

    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private TokenResult HandleNumber()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            while (IsDigit(Peek()))
            {
                Advance();
            }
        }

        return new TokenResult(TokenType.NUMBER, false, double.Parse(_source[_start.._current]));
    }

    private bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               c == '_';
    }

    private bool IsAlphanumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    private TokenResult HandleIdentifier()
    {
        while (IsAlphanumeric(Peek()))
        {
            Advance();
        }

        string text = _source[_start.._current];
        TokenType type = Constants.ReservedKeywords.GetValueOrDefault(text, TokenType.IDENTIFIER);
        return new TokenResult(type, false, text);
    }

    private record TokenResult(TokenType Type, bool IsError, object? Literal = null);
}