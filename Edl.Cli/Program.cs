using Cocona;
using Edl.Core.Ast;
using Edl.Core.Interpreter;
using Edl.Core.Parsing;

class Program
{
    public static void Main(string[] args)
    {
        CoconaApp.Run(Run, args);
    }

    static int Run([Argument] string filename)
    {
        filename = Path.GetFullPath(filename);
        var source = File.ReadAllText(filename);
        var hasParsingErrors = false;
        var expressions = new List<Expression>();
        var l = new Lexer(source, filename);
        var p = new Parser(l);

        foreach (var (expression, errors) in p.Parse())
        {
            if (errors.Length > 0)
            {
                hasParsingErrors = true;
                foreach (var error in errors)
                {
                    Console.Error.WriteLine(error.Message);
                }
            }
            if (expression is not null)
            {
                expressions.Add(expression);
            }
        }

        if (hasParsingErrors) return 1;

        var commands = Translator.Translate(expressions);
        var vm = new Context();

        vm.Execute(commands);

        return 0;
    }
}
