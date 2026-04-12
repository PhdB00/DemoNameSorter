using System.Reflection;
using Microsoft.Extensions.Logging;

namespace DD.NameSorter.Pipeline;

/// <summary>
/// Defines a builder interface for constructing a <see cref="PipelineProcessor"/>.
/// </summary>
public interface IPipelineBuilder
{
    PipelineProcessor Build();
}

/// <summary>
/// An implementation of <see cref="IPipelineBuilder"/>, responsible for constructing
/// and configuring a <see cref="PipelineProcessor"/> using provided pipeline steps.
/// </summary>
/// <remarks>
/// A Pipeline is composed of multiple IPipelineStep objects that are sorted and executed
/// according to their Order PipelineStepOrderAttribute
/// This approach enables us to modify the pipeline as new requirements are discovered
/// without modifying the existing pipeline behaviours and with limited need to 
/// manually rearrange the order of pipeline steps.
/// </remarks>
public class PipelineBuilder(
    IEnumerable<IPipelineStep> steps,
    ILogger<PipelineProcessor> logger)
    : IPipelineBuilder
{
    public PipelineProcessor Build()
    {
        ValidateSteps();
        var pipelineSteps = OrderSteps();
        var pipeline = new PipelineProcessor(pipelineSteps, logger);
        return pipeline;
    }

    private IEnumerable<IPipelineStep> OrderSteps()
    {
        var orderedSteps = steps
            .OrderBy(step => step.GetType()
                .GetCustomAttribute<PipelineStepOrderAttribute>()?.Order ?? int.MaxValue);
        return orderedSteps;
    }
    
    private void ValidateSteps()
    {
        var stepOrders = steps
            .Select(step => step.GetType()
                .GetCustomAttribute<PipelineStepOrderAttribute>())
            .Where(attr => attr != null)
            .Select(attr => attr!.Order)
            .ToList();

        if (stepOrders.Count != stepOrders.Distinct().Count())
        {
            throw new InvalidOperationException("Duplicate step orders found");
        }
    }
}