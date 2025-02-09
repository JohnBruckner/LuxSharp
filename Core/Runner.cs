using System.Text;

namespace Core;

public class Runner
{
    private static bool hadError = false;
    
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

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    private static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
        hadError = true;
    }
}