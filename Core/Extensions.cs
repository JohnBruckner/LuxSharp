namespace Core;

public static class Extensions
{
    public static string Stringify(this object? value)
    {
        switch (value)
        {
            case null:
                return "nil";
            case double:
            {
                var text = value.ToString();
                if (text!.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }

                return text;
            }
            default:
                return value.ToString()!;
        }
    }
}