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
            new LoadCommand("n"),
            new PushCommand(new IntValue(1)),
            new CallCommand("<=", 2),
            new IfCommand(
                trueBranch: [new LoadCommand("n")],
                falseBranch:
                [
                    // fib(n-2)
                    new LoadCommand("n"),
                    new PushCommand(new IntValue(1)),
                    new CallCommand("-", 2),
                    new CallCommand("fib", 1),
                    // fib(n-2)
                    new LoadCommand("n"),
                    new PushCommand(new IntValue(2)),
                    new CallCommand("-", 2),
                    new CallCommand("fib", 1),
                    // + fib(n-1) fib(n-2)
                    new CallCommand("+", 2),
                ]
            )
        };

        var context = new Context();
        context.Execute([
            // Function definition.
            new MakeClosureCommand(["n"], fibBody, default),
            new StoreCommand("fib"),
            // Function call.
            new PushCommand(new IntValue(input, default)),
            new CallCommand("fib", 1),
        ]);
        var result = (IntValue)context.Pop();

        Assert.AreEqual(expectedResult, result.Data);
    }
}
