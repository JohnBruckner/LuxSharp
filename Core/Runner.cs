using System.Text;
using Core.Interpret;
using Core.Parse;
using Core.Syntax;

namespace Core;

public class Runner
{
    private static bool _hadError = false;
    private static bool _hadRuntimeError = false;
    private static Interpreter _interpreter = new Interpreter(RuntimeError);
    
    
    public static void RunFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        // Run(new string(bytes, CharSet.Auto));
        Run(Encoding.Default.GetString(bytes));
    }

    public static void RunPrompt()
    {
        using var reader = new StreamReader(Console.OpenStandardInput());
        while (true)
        {
            Console.Write("> ");
            var line = reader.ReadLine();
            if (line is null)
                break;
            Run(line);
        }
    }

    private static void Run(string source)
    {
        var scanner = new Scanner(source, Error);
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens, Error);
        var statements = parser.Parse();

        if (_hadError) return;
        
        _interpreter.Interpret(statements!);
        
        if (_hadRuntimeError) Environment.Exit(70);
        
        // Console.WriteLine(new AstPrinter().Print(expression!));
    }

    private static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
        _hadError = true;
    }

    private static void Error(Token token, string message)
    {

        switch (token.Type)
        {   
            case TokenType.EOF: Report(token.Line, " at end", message); 
                break;

            default:
                Report(token.Line, $" at '{token.Lexeme}'", message);
                break;
        }
    }

    private static void RuntimeError(int line, string message)
    {
        Console.Error.WriteLine($"{message} \n[line {line}]");
        _hadRuntimeError = true;
    }
}