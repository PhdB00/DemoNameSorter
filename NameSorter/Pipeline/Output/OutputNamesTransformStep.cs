using Microsoft.Extensions.Logging;

namespace DD.NameSorter.Pipeline.Output;

/// <summary>
/// Represents a pipeline step responsible for processing a collection of Person objects through
/// all available output strategies.
/// </summary>
/// <remarks>
/// The OutputNamesTransformStep is an implementation of the IPipelineStep and
/// IPipelineTransformStep interfaces.
/// </remarks>
[PipelineStepOrder(PipelineStepOrders.Output)]
public class OutputNamesTransformStep(
    IEnumerable<IOutputStrategy> outputStrategies,
    ILogger<OutputNamesTransformStep> logger)
    : IPipelineStep, IPipelineTransformStep
{
    public IEnumerable<Person> Process(IEnumerable<Person> people)
    {
        var peopleList = people.ToList();
        logger.LogInformation("Outputting {Count} sorted names using {StrategyCount} strategies",
            peopleList.Count, outputStrategies.Count());

        foreach (var strategy in outputStrategies)
        {
            var strategyName = strategy.GetType().Name;
            logger.LogDebug("Executing output strategy: {StrategyName}", strategyName);
            strategy.Output(peopleList);
        }

        return peopleList;
    }
}