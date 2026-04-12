using DD.NameSorter;
using DD.NameSorter.Pipeline.Output;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace NameSorter.Tests.Pipeline.Output;

[TestFixture]
public class OutputNamesTransformStepTests
{
    [Test]
    public void Process_WithPeople_ShouldOutputToAllStrategies()
    {
        // Arrange
        var consoleStrategy = Substitute.For<IOutputStrategy>();
        var fileStrategy = Substitute.For<IOutputStrategy>();
        var outputStrategies = new List<IOutputStrategy>
        {
            consoleStrategy, fileStrategy
        };
        var step = new OutputNamesTransformStep(outputStrategies, NullLogger<OutputNamesTransformStep>.Instance);
        var people = new List<Person>
        {
            new(new[] { "John" }, "Smith"),
            new(new[] { "Jane" }, "Doe")
        };
        
        // Act
        step.Process(people);
        
        // Assert
        consoleStrategy.Received().Output(Arg.Any<IEnumerable<Person>>());
        fileStrategy.Received().Output(Arg.Any<IEnumerable<Person>>());
    }
}