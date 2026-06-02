using Edl.Core.Parsing;

namespace Edl.Tests;

[TestClass]
public class ParserTests
{
    [TestMethod]
    public void EmptyFunctionBody()
    {
        var parser = new Parser("let foo \\x\\ {\n}", "test.edl");
        var nodes = parser.Parse().ToArray();

        Assert.HasCount(1, nodes);
        Assert.IsInstanceOfType<Call>(nodes[0].Item1);

        var call = (Call)nodes[0].Item1!;
        Assert.AreEqual("let", call.Function.Name);
        Assert.HasCount(2, call.Arguments);
        Assert.IsInstanceOfType<Identifier>(call.Arguments[0]);
        Assert.IsInstanceOfType<Function>(call.Arguments[1]);

        var name = (Identifier)call.Arguments[0];
        var body = (Function)call.Arguments[1];
        Assert.AreEqual("foo", name.Name);
        Assert.HasCount(1, body.Parameters);
        Assert.HasCount(0, body.Body);
        Assert.AreEqual("x", body.Parameters[0].Name);
    }
}
