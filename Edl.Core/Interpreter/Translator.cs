using Edl.Core.Ast;

namespace Edl.Core.Interpreter;

public readonly struct Translator()
{
    private static readonly Dictionary<string, IMacro> MACROS = Registry.Load<IMacro, MacroAttribute>();

    public readonly List<ICommand> Commands { get; } = [];
    public readonly Dictionary<string, IMacro> Macros { get; init; } = MACROS;
    // public Dictionary<string, Func<LineInfo, ICommand>> Intrinsics { get; init; } = [];

    public static List<ICommand> Translate(IList<Expression> expressions)
    {
        var translator = new Translator();
        foreach (var expression in expressions) translator.Translate(expression);
        return translator.Commands;
    }

    public void Translate(Expression expression)
    {
        switch (expression)
        {
        case IntLiteral i:
            Commands.Add(new PushCommand(new IntValue(i.Value, i.Location)));
            break;

        case FloatLiteral f:
            Commands.Add(new PushCommand(new FloatValue(f.Value, f.Location)));
            break;

        case StringLiteral s:
            Commands.Add(new PushCommand(new StringValue(s.Data, s.Location)));
            break;

        case Identifier id:
            Commands.Add(new LoadCommand(id.Name));
            break;

        case Call call:
            if (Macros.TryGetValue(call.Function.Name, out var macro))
            {
                macro.Translate(call, this);
            }
            else
            {
                foreach (var arg in call.Arguments)
                {
                    Translate(arg);
                }
                // Commands.Add(
                //     Intrinsics.TryGetValue(call.Function.Name, out var factory)
                //         ? new DispatchCommand(call.Function.Name, factory(call.Function.Location))
                //         : new CallCommand(call.Function.Name, call.Arguments.Length)
                // );
                Commands.Add(new CallCommand(call.Function.Name, call.Arguments.Length));
            }
            break;

        case Function func:
            Commands.Add(new MakeClosureCommand(
                func.Parameters.Select(p => p.Name).ToArray(),
                Translate(func.Body).ToArray(),
                func.Location
            ));
            break;

        case ExpressionList list:
            foreach (var item in list.Items)
            {
                Translate(item);
            }
            Commands.Add(new MakeListCommand(list.Items.Length, list.Location));
            break;

        default:
            throw new NotImplementedException($"Translation of {expression.GetType().Name} is not implemented.");
        }
    }
}
