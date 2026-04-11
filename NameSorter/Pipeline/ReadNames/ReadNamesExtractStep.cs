using DD.NameSorter.Configuration;
using DD.NameSorter.Infrastructure;

namespace DD.NameSorter.Pipeline.ReadNames;

/// <summary>
/// Represents a pipeline step responsible for reading and extracting strings,
/// representing person names, from an input file and returning them as a collection of Person.
/// </summary>
[PipelineStepOrder(PipelineStepOrders.Read)]
public class ReadNamesExtractStep(
    ICommandLineConfig config,
    IFileSystem fileSystem,
    INameParser nameParser)
    : IPipelineStep, IPipelineExtractStep
{
    public IEnumerable<Person> Process()
    {
        var filePath = config.InputFile;
        
        if (!fileSystem.Exists(filePath))
            throw new FileNotFoundException("Input file not found.", filePath);

        var names = fileSystem.ReadAllLines(filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line.Trim()));
        
        var results = names.Select(nameParser.ParseName).ToList();
        
        var errors = results
            .Where(r => !r.IsSuccess)
            .Select(r => r.ErrorMessage)
            .Where(msg => msg != null)
            .ToList();

        if (errors.Any())
        {
            throw new ArgumentException(
                $"Failed to parse some names in {filePath}{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
        }

        return results.Select(r => r.Value!);
    }
}
