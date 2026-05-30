using Edl.Core.Interpreter;

namespace Edl.Tests;

[TestClass]
public class ExecutionTests
{
    [TestMethod]
    [DataRow(0L, 0L)]
    [DataRow(1L, 1L)]
    [DataRow(2L, 1L)]
    [DataRow(5L, 5L)]
    [DataRow(10L, 55L)]
    [DataRow(15L, 610L)]
    [DataRow(30L, 832040L)]
    public void Fibonacci(long input, long expectedResult)
    {
        var fibBody = new ICommand[]
        {
            new LoadCommand("<="),
            new LoadCommand("n"),
            new PushCommand(new IntValue(1)),
            new CallCommand(2),
            new IfCommand(
                trueBranch: [new LoadCommand("n")],
                falseBranch:
                [
                    new LoadCommand("+"),
                    // fib(n-2)
                    new LoadCommand("fib"),
                    new LoadCommand("-"),
                    new LoadCommand("n"),
                    new PushCommand(new IntValue(1)),
                    new CallCommand(2),
                    new CallCommand(1),
                    // fib(n-2)
                    new LoadCommand("fib"),
                    new LoadCommand("-"),
                    new LoadCommand("n"),
                    new PushCommand(new IntValue(2)),
                    new CallCommand(2),
                    new CallCommand(1),
                    // + fib(n-1) fib(n-2)
                    new CallCommand(2),
                ]
            )
        };

        var context = new Context();
        context.Execute([
            // Function definition.
            new MakeClosureCommand(["n"], [], fibBody, default),
            new StoreCommand("fib"),
            // Function call.
            new PushCommand(new IntValue(input, default)),
            new CallCommand(1),
        ]);
        var result = (IntValue)context.Pop();

        Assert.AreEqual(expectedResult, result.Data);
    }

    [TestMethod]
    public void PrintIntrinsicReturnsZero()
    {
        var context = new Context();
        context.Execute([
            new LoadCommand("print"),
            new PushCommand(new StringValue("hi", default)),
            new CallCommand(1),
        ]);

        var result = (IntValue)context.Pop();
        Assert.AreEqual(0L, result.Data);
    }

    // [TestMethod]
    // public void TailRecursiveFunctionUsesTailCallOptimization()
    // {
    //     var countBody = new ICommand[]
    //     {
    //         new LoadCommand("n"),
    //         new PushCommand(new IntValue(0)),
    //         new CallCommand("<=", 2),
    //         new IfCommand(
    //             trueBranch: [new PushCommand(new IntValue(0))],
    //             falseBranch: [new LoadCommand("n"), new PushCommand(new IntValue(1)), new CallCommand("-", 2), new TailCallCommand("count", 1)]
    //         )
    //     };

    //     var context = new Context();
    //     context.Execute([
    //         new MakeClosureCommand(["n"], countBody, default),
    //         new StoreCommand("count"),
    //         new PushCommand(new IntValue(10000, default)),
    //         new CallCommand("count", 1),
    //     ]);

    //     var result = (IntValue)context.Pop();
    //     Assert.AreEqual(0L, result.Data);
    // }
}
