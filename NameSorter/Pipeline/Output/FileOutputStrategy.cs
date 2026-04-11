using DD.NameSorter.Configuration;
using DD.NameSorter.Infrastructure;

namespace DD.NameSorter.Pipeline.Output;

/// <summary>
/// FileOutputStrategy implements the <see cref="IOutputStrategy"/> 
/// and is responsible for outputting the names of a collection of Person objects to a file.
/// </summary>
/// /// <remarks>
/// This class utilizes an <see cref="IFileSystem"/> to abstract file writing, making
/// it easier to unit test and allowing for dependency injection of custom file writers.
/// </remarks>
public class FileOutputStrategy(
    ICommandLineConfig config,
    IFileSystem fileSystem) 
    : IOutputStrategy
{
    public void Output(IEnumerable<Person> people)
    {
        if (!people.Any())
            return;
        
        string filePath = config.OutputFile ?? throw new ArgumentNullException(nameof(config.OutputFile));
        fileSystem.WriteAllLines(filePath, people.Select(p => p.ToString()));
    }
}