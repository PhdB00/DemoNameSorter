using DD.NameSorter.Abstractions;

namespace DD.NameSorter.Pipeline.ReadNames;

/// <summary>
/// Defines a contract for parsing a string, represeting a full name, into a <see cref="Person"/> object.
/// </summary>
public interface INameParser
{
    Result<Person> ParseName(string fullName);
}

/// <summary>
/// Implements the <see cref="INameParser"/> interface to parse a string into a <see cref="Person"/>.
/// </summary>
/// <remarks>
/// Parses the input string to separate given names and the last name.
/// Ensures the name follows requirements such as containing at least two parts (a given name and a last name),
/// and restricts the total number of parts that may comprise a given name.
/// </remarks>
public class NameParser : INameParser
{
    public Result<Person> ParseName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return Result<Person>.Failure("Full name cannot be empty.");

        var nameParts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        switch (nameParts.Length)
        {
            case < 2:
                return Result<Person>.Failure($"Name must contain at least one given name and one last name ({fullName}).");
            
            case > 4:
                return Result<Person>.Failure($"Name cannot contain more than three given names and one last name ({fullName}).");
            
            default:
                var lastName = nameParts[^1];
                var givenNames = nameParts[..^1].ToList();

                return Result<Person>.Success(new Person(givenNames, lastName));
        }
    }
}
