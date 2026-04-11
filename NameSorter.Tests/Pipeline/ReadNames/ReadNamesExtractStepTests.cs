using DD.NameSorter;
using DD.NameSorter.Abstractions;
using DD.NameSorter.Configuration;
using DD.NameSorter.Infrastructure;
using DD.NameSorter.Pipeline.ReadNames;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace NameSorter.Tests.Pipeline.ReadNames;

[TestFixture]
public class ReadNamesExtractStepTests
{
    private ICommandLineConfig config;
    private IFileSystem fileSystem;
    private INameParser nameParser;
    private ReadNamesExtractStep step;

    [SetUp]
    public void Setup()
    {
        config = Substitute.For<ICommandLineConfig>();
        fileSystem = Substitute.For<IFileSystem>();
        nameParser = Substitute.For<INameParser>();
        step = new ReadNamesExtractStep(config, fileSystem, nameParser);
    }

    [Test]
    public void Process_WithValidFile_ReturnsParsedNames()
    {
        // Arrange
        var filePath = "test.txt";
        var fileLines = new[] { "John Smith", "Jane Doe", "Bob Johnson" };
            
        var expectedPeople = new[]
        {
            new Person(["John"], "Smith"),
            new Person(["Jane"], "Doe"),
            new Person(["Bob"], "Johnson")
        };

        config.InputFile.Returns(filePath);
        fileSystem.Exists(filePath).Returns(true);
        fileSystem.ReadAllLines(filePath).Returns(fileLines);

        for (int i = 0; i < fileLines.Length; i++)
        {
            nameParser.ParseName(fileLines[i]).Returns(Result<Person>.Success(expectedPeople[i]));
        }

        // Act
        var result = step.Process().ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result, Is.EqualTo(expectedPeople));
        });
            
        fileSystem.Received(1).Exists(filePath);
        fileSystem.Received(1).ReadAllLines(filePath);
        nameParser.Received(3).ParseName(Arg.Any<string>());
    }

    [Test]
    public void Process_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var filePath = "nonexistent.txt";
        config.InputFile.Returns(filePath);
        fileSystem.Exists(filePath).Returns(false);

        // Act, Assert
        var ex = Assert.Throws<FileNotFoundException>(() => step.Process().ToList());
        Assert.Multiple(() =>
        {
            Assert.That(ex.Message, Does.Contain("Input file not found"));
            Assert.That(ex.FileName, Is.EqualTo(filePath));
        });
    }

    [Test]
    public void Process_WithEmptyFile_ReturnsEmptyCollection()
    {
        // Arrange
        var filePath = "empty.txt";
        var fileLines = Array.Empty<string>();

        config.InputFile.Returns(filePath);
        fileSystem.Exists(filePath).Returns(true);
        fileSystem.ReadAllLines(filePath).Returns(fileLines);

        // Act
        var result = step.Process().ToList();

        // Assert
        Assert.That(result, Is.Empty);
        nameParser.DidNotReceive().ParseName(Arg.Any<string>());
    }

    [Test]
    public void Process_WithBlankLines_SkipsBlankLines()
    {
        // Arrange
        var filePath = "test.txt";
        var fileLines = new[] { "John Smith", "", "   ", "Jane Doe" };
            
        var expectedPeople = new[]
        {
            new Person(["John"], "Smith"),
            new Person(["Jane"], "Doe")
        };

        config.InputFile.Returns(filePath);
        fileSystem.Exists(filePath).Returns(true);
        fileSystem.ReadAllLines(filePath).Returns(fileLines);

        nameParser.ParseName(fileLines[0]).Returns(Result<Person>.Success(expectedPeople[0]));
        nameParser.ParseName(fileLines[3]).Returns(Result<Person>.Success(expectedPeople[1]));

        // Act
        var result = step.Process().ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result, Is.EqualTo(expectedPeople));
        });
            
        nameParser.Received(2).ParseName(Arg.Any<string>());
    }

    [Test]
    public void Process_WhenParserThrowsException_PropagatesException()
    {
        // Arrange
        var filePath = "test.txt";
        var fileLines = new[] { "Invalid Name" };
        var expectedError = new ArgumentException("Invalid name format");

        config.InputFile.Returns(filePath);
        fileSystem.Exists(filePath).Returns(true);
        fileSystem.ReadAllLines(filePath).Returns(fileLines);
        nameParser.ParseName(Arg.Any<string>()).Throws(expectedError);

        // Act, Assert
        var ex = Assert.Throws<ArgumentException>(() => step.Process().ToList());
        Assert.That(ex, Is.EqualTo(expectedError));
    }

    [Test]
    public void Process_WhenFileSystemThrowsException_PropagatesException()
    {
        // Arrange
        var filePath = "test.txt";
        var expectedError = new IOException("File system error");

        config.InputFile.Returns(filePath);
        fileSystem.Exists(filePath).Returns(true);
        fileSystem.ReadAllLines(filePath).Throws(expectedError);

        // Act, Assert
        var ex = Assert.Throws<IOException>(() => step.Process().ToList());
        Assert.That(ex, Is.EqualTo(expectedError));
    }
}