using Edl.Core.Interpreter;
using Edl.Core.Interpreter.Intrinsics.Arithmetics;
using Edl.Core.Interpreter.Intrinsics.Comparison;

namespace Edl.Tests;

[TestClass]
public class ExecutionTests
{
    private static IEnumerable<ICommand> GetFibonacciSetupCommands()
    {
        var fibBody = new ICommand[]
        {
            new LoadCommand("n"),
            new PushCommand(new IntValue(2)),
            new LessThanCommand(),
            new IfCommand(
                trueBranch:
                [
                    new LoadCommand("n")
                ],
                falseBranch:
                [
                    new LoadCommand("n"),
                    new PushCommand(new IntValue(1)),
                    new SubtractCommand(),
                    new CallCommand("fib", 1),

                    new LoadCommand("n"),
                    new PushCommand(new IntValue(2)),
                    new SubtractCommand(),
                    new CallCommand("fib", 1),

                    new AddCommand()
                ]
            )
        };
        return
        [
            new MakeClosureCommand(["n"], fibBody, default),
            new StoreCommand("fib")
        ];
    }

    [TestMethod]
    public void Fibonacci10()
    {
        var context = new Core.Interpreter.Context();
        context.Execute(GetFibonacciSetupCommands());

        var testCommands = new ICommand[]
        {
            new PushCommand(new IntValue(10)),
            new CallCommand("fib", 1)
        };
        context.Execute(testCommands);
        var result = (IntValue)context.Pop();

        Assert.AreEqual(55, result.Data);
    }

    [TestMethod]
    [DataRow(0L, 0L)]
    [DataRow(1L, 1L)]
    [DataRow(2L, 1L)]
    [DataRow(5L, 5L)]
    [DataRow(10L, 55L)]
    [DataRow(15L, 610L)]
    public void Fibonacci_VariousInputs_ReturnsCorrectSequence(long input, long expectedResult)
    {
        var vm = new Core.Interpreter.Context();
        vm.Execute(GetFibonacciSetupCommands());

        var testCommands = new ICommand[]
        {
            new PushCommand(new IntValue(input, default)),
            new CallCommand("fib", 1)
        };

        vm.Execute(testCommands);
        var result = (IntValue)vm.Pop();

        Assert.AreEqual(expectedResult, result.Data);
    }

    [TestMethod]
    public void FunctionScopeCapture()
    {
        var vm = new Core.Interpreter.Context();
        vm.Execute(
        [
        ]);
    }
}
