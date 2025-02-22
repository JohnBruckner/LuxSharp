using Core.Exceptions;

namespace Core.Env;

public class LoxEnvironment()
{
    private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

    public void Define(string name, object value)
    {
        _values.Add(name, value);
    }

    public object? Get(Token name)
    {
        if (_values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Assign(Token name, object value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }
}