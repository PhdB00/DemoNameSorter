using DD.NameSorter.Pipeline.ReadNames;

namespace NameSorter.Tests.Pipeline.ReadNames;

[TestFixture]
public class NameParserTests
{
    private NameParser parser;

    [SetUp]
    public void Setup()
    {
        parser = new NameParser();
    }

    [Test]
    public void ParseName_WithValidTwoPartName_ReturnsPerson()
    {
        // Arrange
        var fullName = "John Smith";

        // Act
        var result = parser.ParseName(fullName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.GivenNames, Has.Count.EqualTo(1));
            Assert.That(result.Value!.GivenNames[0], Is.EqualTo("John"));
            Assert.That(result.Value!.LastName, Is.EqualTo("Smith"));
        });
    }

    [Test]
    public void ParseName_WithValidThreePartName_ReturnsPerson()
    {
        // Arrange
        var fullName = "John Robert Smith";

        // Act
        var result = parser.ParseName(fullName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.GivenNames, Has.Count.EqualTo(2));
            Assert.That(result.Value!.GivenNames[0], Is.EqualTo("John"));
            Assert.That(result.Value!.GivenNames[1], Is.EqualTo("Robert"));
            Assert.That(result.Value!.LastName, Is.EqualTo("Smith"));
        });
    }

    [Test]
    public void ParseName_WithValidFourPartName_ReturnsPerson()
    {
        // Arrange
        var fullName = "John Robert James Smith";

        // Act
        var result = parser.ParseName(fullName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.GivenNames, Has.Count.EqualTo(3));
            Assert.That(result.Value!.GivenNames[0], Is.EqualTo("John"));
            Assert.That(result.Value!.GivenNames[1], Is.EqualTo("Robert"));
            Assert.That(result.Value!.GivenNames[2], Is.EqualTo("James"));
            Assert.That(result.Value!.LastName, Is.EqualTo("Smith"));
        });
    }
        
    [TestCase("")]
    [TestCase("   ")]
    public void ParseName_WithEmptyInput_ReturnsFalse(string input)
    {
        // Act
        var result = parser.ParseName(input);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Value, Is.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Full name cannot be empty"));
        });
    }

    [TestCase("John")]
    [TestCase("A")]
    public void ParseName_WithSingleName_RetursFalse(string input)
    {
        // Act
        var result = parser.ParseName(input);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Value, Is.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Name must contain at least one given name and one last name"));
        });
    }

    [Test]
    public void ParseName_WithMoreThanFourParts_ReturnsFalse()
    {
        // Arrange
        var fullName = "John Robert James William Smith";

        // Act
        var result = parser.ParseName(fullName);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Value, Is.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Name cannot contain more than three given names and one last name"));
        });
    }

    [Test]
    public void ParseName_WithExtraSpaces_TrimsAndParsesProperly()
    {
        // Arrange
        var fullName = "   John   Robert    Smith   ";

        // Act
        var result = parser.ParseName(fullName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.GivenNames, Has.Count.EqualTo(2));
            Assert.That(result.Value!.GivenNames[0], Is.EqualTo("John"));
            Assert.That(result.Value!.GivenNames[1], Is.EqualTo("Robert"));
            Assert.That(result.Value!.LastName, Is.EqualTo("Smith"));
        });
    }

    [Test]
    public void ParseName_WithTwoPartsAndExtraSpaces_TrimsAndParsesProperly()
    {
        // Arrange
        var fullName = "   John    Smith   ";

        // Act
        var result = parser.ParseName(fullName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.GivenNames, Has.Count.EqualTo(1));
            Assert.That(result.Value!.GivenNames[0], Is.EqualTo("John"));
            Assert.That(result.Value!.LastName, Is.EqualTo("Smith"));
        });
    }
}