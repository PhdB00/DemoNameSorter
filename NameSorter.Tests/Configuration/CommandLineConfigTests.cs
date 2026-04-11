using DD.NameSorter.Configuration;

namespace NameSorter.Tests.Configuration;

[TestFixture]
public class CommandLineConfigTests
{
    [Test]
    public void Constructor_WithValidInputFile_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var config = new CommandLineConfig(["test.txt"]);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(config.InputFile, Is.EqualTo("test.txt"));
            Assert.That(config.IsValid, Is.True);
        });
    }

    [Test]
    public void Constructor_WithEmptyArgs_IsNotValid()
    {
        // Arrange & Act
        var config = new CommandLineConfig([]);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(config.IsValid, Is.False);
            Assert.That(config.InputFile, Is.Null);
        });
    }

    [Test]
    public void Constructor_WithNullArgs_IsNotValid()
    {
        // Arrange & Act
        var config = new CommandLineConfig(null!);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(config.IsValid, Is.False);
            Assert.That(config.InputFile, Is.Null);
        });
    }

    [Test]
    public void Constructor_WithTooManyArguments_IsNotValid()
    {
        // Arrange & Act
        var config = new CommandLineConfig(["file1.txt", "file2.txt"]);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(config.IsValid, Is.False);
            Assert.That(config.InputFile, Is.Null);
        });
    }

    [Test]
    public void Constructor_WithEmptyFileName_IsNotValid()
    {
        // Arrange, Act
        var config = new CommandLineConfig([""]);

        // Assert
        Assert.That(config.IsValid, Is.False);
    }

    [Test]
    public void Constructor_WithWhiteSpaceFileName_IsNotValid()
    {
        // Arrange & Act
        var config = new CommandLineConfig([" "]);

        // Assert
        Assert.That(config.IsValid, Is.False);
    }

    [TestCase("file.txt")]
    [TestCase("path/to/file.txt")]
    [TestCase(@"C:\path\to\file.txt")]
    [TestCase("../file.txt")]
    public void Constructor_WithValidFilePaths_IsValid(string filePath)
    {
        // Arrange & Act
        var config = new CommandLineConfig([filePath]);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(config.IsValid, Is.True);
            Assert.That(config.InputFile, Is.EqualTo(filePath));
        });
    }
}