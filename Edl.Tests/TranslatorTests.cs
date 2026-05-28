using Edl.Core.Ast;
using Edl.Core.Interpreter;
using Edl.Core.Parsing;

namespace Edl.Tests;

[TestClass]
public class TranslatorTests
{
    private readonly LineInfo _dummyLocation = new("test.src", 1, 1);

    [TestMethod]
    public void Translate_IntLiteral()
    {
        var literal = new IntLiteral { Value = 42, Location = _dummyLocation };
        var commands = Translator.Translate([literal]);

        Assert.HasCount(1, commands);
        Assert.IsInstanceOfType<PushCommand>(commands[0]);

        var pushCommand = (PushCommand)commands[0];
        Assert.IsInstanceOfType<IntValue>(pushCommand.Value);

        var intValue = (IntValue)pushCommand.Value;
        Assert.AreEqual(42L, intValue.Data);
        Assert.AreEqual(_dummyLocation, intValue.Origin);
    }

    [TestMethod]
    public void Translate_Call_Plus()
    {
        var commands = Translator.Translate(
        [
            new Call
            {
                Function = new Identifier { Name = "+", Location = _dummyLocation },
                Arguments =
                [
                    new IntLiteral { Value = 10, Location = new LineInfo("test.src", 1, 1) },
                    new IntLiteral { Value = 20, Location = new LineInfo("test.src", 1, 5) },
                ],
                Location = _dummyLocation
            }
        ]);

        Assert.HasCount(3, commands);
        Assert.IsInstanceOfType<PushCommand>(commands[0]);
        Assert.AreEqual(10L, ((IntValue)((PushCommand)commands[0]).Value).Data);
        Assert.IsInstanceOfType<PushCommand>(commands[1]);
        Assert.AreEqual(20L, ((IntValue)((PushCommand)commands[1]).Value).Data);
        Assert.IsInstanceOfType<CallCommand>(commands[2]);

        var callCommand = (CallCommand)commands[2];
        Assert.AreEqual("+", callCommand.FunctionName);
        Assert.AreEqual(2, callCommand.ArgCount);
        // Assert.AreEqual(_dummyLocation, dispatchCommand.Location);
    }

    [TestMethod]
    public void Translate_ExpressionList_GeneratesCreateListCommand()
    {
        var commands = Translator.Translate(
        [
            new ExpressionList
            {
                Items =
                [
                    new IntLiteral { Value = 1, Location = _dummyLocation },
                    new IntLiteral { Value = 2, Location = _dummyLocation },
                    new IntLiteral { Value = 3, Location = _dummyLocation },
                ],
                Location = _dummyLocation
            }
        ]);

        Assert.HasCount(4, commands);
        for (int i = 0; i < 3; i++) Assert.IsInstanceOfType<PushCommand>(commands[i]);
        Assert.IsInstanceOfType<MakeListCommand>(commands[3]);

        var makeListCommand = (MakeListCommand)commands[3];
        Assert.AreEqual(3, makeListCommand.ElementCount);
        // Assert.AreEqual(_dummyLocation, createListCommand.Location);
    }
}
