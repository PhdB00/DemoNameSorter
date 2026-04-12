using DD.NameSorter.Configuration;
using DD.NameSorter.Infrastructure;
using Microsoft.Extensions.Logging;

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
    IFileSystem fileSystem,
    ILogger<FileOutputStrategy> logger)
    : IOutputStrategy
{
    public void Output(IEnumerable<Person> people)
    {
        if (!people.Any())
        {
            logger.LogWarning("No names to output to file");
            return;
        }

        string filePath = config.OutputFile ?? throw new ArgumentNullException(nameof(config.OutputFile));
        logger.LogInformation("Writing {Count} sorted names to file {FilePath}", people.Count(), filePath);

        try
        {
            fileSystem.WriteAllLines(filePath, people.Select(p => p.ToString()));
            logger.LogInformation("Successfully wrote sorted names to {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to write sorted names to file {FilePath}", filePath);
            throw;
        }
    }
}