using System.Text;
using Core;
using Core.Syntax;
using Core.Tools;

namespace LoxSharp;

class Program
{
    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: jlox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            Runner.RunFile(args[0]);
        }
        else
        {
            Runner.RunPrompt();
        }
    }
}