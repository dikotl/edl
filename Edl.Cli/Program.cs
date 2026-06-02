using System.Diagnostics;
using Cocona;
using Edl.Core.Parsing;
using Edl.Core.Translation;
using Edl.Core.VirtualMachine;

class Program
{
    public static void Main(string[] args)
    {
        CoconaApp.Run(Run, args);
    }

    static int Run([Argument] string filename, [Option("time", ['t'])] bool? time)
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
        var context = new Context();

        var watch = new Stopwatch();
        if (time is true)
        {
            watch.Start();
        }

        context.Execute(commands);

        if (time is true)
        {
            watch.Stop();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Error.WriteLine($"execution time: {watch.Elapsed:mm\\:ss\\.fff}");
            Console.ResetColor();
        }

        return 0;
    }
}
