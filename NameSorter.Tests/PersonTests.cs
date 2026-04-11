using DD.NameSorter;

namespace NameSorter.Tests;

public class PersonTests
{
    [Test]
    public void Constructor_WithValidInputs_CreatesPersonSuccessfully()
    {
        // Arrange
        var givenNames = new[] { "John", "Robert" };
        var lastName = "Smith";

        // Act
        var person = new Person(givenNames, lastName);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(person.GivenNames, Is.EqualTo(givenNames));
            Assert.That(person.LastName, Is.EqualTo(lastName));
        });
    }

    [Test]
    public void Constructor_WithNullGivenNames_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<string>? givenNames = null;
        var lastName = "Smith";

        // Act, Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new Person(givenNames!, lastName));
        Assert.That(ex.ParamName, Is.EqualTo("givenNames"));
    }

    [Test]
    public void Constructor_WithNullLastName_ThrowsArgumentNullException()
    {
        // Arrange
        var givenNames = new[] { "John" };
        string? lastName = null;

        // Act, Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new Person(givenNames, lastName!));
        Assert.That(ex.ParamName, Is.EqualTo("lastName"));
    }

    [Test]
    public void Constructor_WithEmptyGivenNames_ThrowsArgumentException()
    {
        // Arrange
        var givenNames = Array.Empty<string>();
        var lastName = "Smith";

        // Act, Assert
        Assert.Multiple(() =>
        {
            var ex = Assert.Throws<ArgumentException>(() => new Person(givenNames, lastName));
            Assert.That(ex.ParamName, Is.EqualTo("givenNames"));
            Assert.That(ex.Message, Does.Contain("At least one given name is required"));
        });
    }

    [Test]
    public void Constructor_WithMoreThanThreeGivenNames_ThrowsArgumentException()
    {
        // Arrange
        var givenNames = new[] { "John", "Robert", "James", "William" };
        var lastName = "Smith";

        // Act, Assert
        Assert.Multiple(() =>
        {
            var ex = Assert.Throws<ArgumentException>(() => new Person(givenNames, lastName));
            Assert.That(ex.ParamName, Is.EqualTo("givenNames"));
            Assert.That(ex.Message, Does.Contain("Maximum three given names are allowed"));
        });
    }

    [Test]
    public void Constructor_WithWhiteSpaceLastName_ThrowsArgumentNullException()
    {
        // Arrange
        var givenNames = new[] { "John" };
        var lastName = "   ";

        // Act, Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new Person(givenNames, lastName));
        Assert.That(ex.ParamName, Is.EqualTo("lastName"));
    }

    [Test]
    public void ToString_ReturnsCorrectlyFormattedString()
    {
        // Arrange
        var givenNames = new[] { "John", "Robert" };
        var lastName = "Smith";
        var person = new Person(givenNames, lastName);

        // Act
        var result = person.ToString();

        // Assert
        Assert.That(result, Is.EqualTo("John Robert Smith"));
    }

    [TestCase(new[] { "Mary" }, "Jones", "Mary Jones")]
    [TestCase(new[] { "Mary", "Jane" }, "Jones", "Mary Jane Jones")]
    [TestCase(new[] { "Mary", "Jane", "Ann" }, "Jones", "Mary Jane Ann Jones")]
    public void ToString_WithDifferentNameCombinations_ReturnsCorrectFormat(
        string[] givenNames, string lastName, string expected)
    {
        // Arrange
        var person = new Person(givenNames, lastName);

        // Act
        var result = person.ToString();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}