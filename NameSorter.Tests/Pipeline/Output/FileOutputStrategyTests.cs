using DD.NameSorter;
using DD.NameSorter.Configuration;
using DD.NameSorter.Infrastructure;
using DD.NameSorter.Pipeline.Output;
using NSubstitute;

namespace NameSorter.Tests.Pipeline.Output;

[TestFixture]
public class FileOutputStrategyTests
{
    [Test]
    public void Output_WithNoNames_WritesNothing()
    {
        // Arrange
        var commandLine = Substitute.For<ICommandLineConfig>();
        var fileSystem = Substitute.For<IFileSystem>();
        var fileOutput = new FileOutputStrategy(commandLine, fileSystem);
        
        commandLine.OutputFile.Returns("test-output.txt");
        
        // Act
        fileOutput.Output((List<Person>) []);
        
        // Assert
        fileSystem.DidNotReceive().WriteAllLines("test-output.txt", Arg.Any<IEnumerable<string>>());
    }

    [Test]
    public void Output_WithNames_WritesEachName()
    {
        // Arrange
        var commandLine = Substitute.For<ICommandLineConfig>();
        var fileSystem = Substitute.For<IFileSystem>();
        var fileOutput = new FileOutputStrategy(commandLine, fileSystem);
        var names = new List<Person>
        {
            new(["John"], "Smith"),
            new(["Jane"], "Doe")
        };
        
        commandLine.OutputFile.Returns("test-output.txt");
        
        // Act
        fileOutput.Output(names);
        
        // Assert
        fileSystem.Received().WriteAllLines("test-output.txt", 
            Arg.Any<IEnumerable<string>>());
    }
}