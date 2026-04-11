using DD.NameSorter;

namespace NameSorter.Tests.Pipeline.SortNames;

[TestFixture]
public class NameSorterTests
{
    private DD.NameSorter.Pipeline.SortNames.NameSorter sorter;

    [SetUp]
    public void Setup()
    {
        sorter = new DD.NameSorter.Pipeline.SortNames.NameSorter();
    }

    [Test]
    public void Sort_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var people = new List<Person>();

        // Act
        var result = sorter.Sort(people).ToList();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Sort_WithDifferentLastNames_SortsByLastName()
    {
        // Arrange
        var people = new List<Person>
        {
            new(["John"], "Smith"),
            new(["Jane"], "Doe"),
            new(["Bob"], "Anderson")
        };

        // Act
        var result = sorter.Sort(people).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0].LastName, Is.EqualTo("Anderson"));
            Assert.That(result[1].LastName, Is.EqualTo("Doe"));
            Assert.That(result[2].LastName, Is.EqualTo("Smith"));
        });
    }

    [Test]
    public void Sort_WithSameLastNames_SortsByGivenNames()
    {
        // Arrange
        var people = new List<Person>
        {
            new Person(new[] { "John" }, "Smith"),
            new Person(new[] { "Adam" }, "Smith"),
            new Person(new[] { "Bob" }, "Smith")
        };

        // Act
        var result = sorter.Sort(people).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0].GivenNames[0], Is.EqualTo("Adam"));
            Assert.That(result[1].GivenNames[0], Is.EqualTo("Bob"));
            Assert.That(result[2].GivenNames[0], Is.EqualTo("John"));
        });
    }

    [Test]
    public void Sort_WithMultipleGivenNames_SortsByAllGivenNames()
    {
        // Arrange
        var people = new List<Person>
        {
            new Person(new[] { "John", "Adam" }, "Smith"),
            new Person(new[] { "John", "Bob" }, "Smith"),
            new Person(new[] { "John", "Albert" }, "Smith")
        };

        // Act
        var result = sorter.Sort(people).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(string.Join(" ", result[0].GivenNames), Is.EqualTo("John Adam"));
            Assert.That(string.Join(" ", result[1].GivenNames), Is.EqualTo("John Albert"));
            Assert.That(string.Join(" ", result[2].GivenNames), Is.EqualTo("John Bob"));
        });
    }
}