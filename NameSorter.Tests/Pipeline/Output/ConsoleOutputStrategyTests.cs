using DD.NameSorter;
using DD.NameSorter.Infrastructure;
using DD.NameSorter.Pipeline.Output;
using NSubstitute;

namespace NameSorter.Tests.Pipeline.Output;

[TestFixture]
public class ConsoleOutputStrategyTests
{
    [Test]
    public void Write_WithNoNames_WritesNothing()
    {
        // Arrange
        var consoleWriter = Substitute.For<IConsoleWriter>();
        var output = new ConsoleOutputStrategy(consoleWriter);
        
        // Act
        output.Output((List<Person>) []);
        
        // Assert
        consoleWriter.DidNotReceive().WriteLine(Arg.Any<string>());
    }
    
    [Test]
    public void Write_WithNames_WritesEachName()
    {
        // Arrange
        var consoleWriter = Substitute.For<IConsoleWriter>();
        var output = new ConsoleOutputStrategy(consoleWriter);
        var names = new List<Person>
        {
            new(["John"], "Smith"),
            new(["Jane"], "Doe")
        };
        
        // Act
        output.Output(names);
        
        // Assert
        consoleWriter.Received(2).WriteLine(Arg.Any<string>());
        var calls = consoleWriter.ReceivedCalls().ToList();
        Assert.Multiple(() =>
        {
            Assert.That(calls[0].GetArguments()[0], Is.EqualTo("John Smith"));
            Assert.That(calls[1].GetArguments()[0], Is.EqualTo("Jane Doe"));
        });
    }
}