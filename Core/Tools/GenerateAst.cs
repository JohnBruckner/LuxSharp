namespace Core.Tools;

public static class GenerateAst
{
    public static void GenAst(string[] args)
    {
        string outputDir = args[0];
        DefineAst(outputDir, "Expr",
        [
            "Binary : Expr left, Token operator, Expr right", 
            "Grouping : Expr expression", "Literal : object value",
            "Unary : Token operator, Expr right"
        ]);
    }

    private static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        string output = Path.Combine(outputDir, baseName, ".cs");
        
        
    }
}