using DD.NameSorter.Configuration;
using DD.NameSorter.Pipeline;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DD.NameSorter;

/// <summary>
/// A hosted service that represents the primary execution pipeline for the application.
/// </summary>
/// <remarks>
/// This service will validate the command-line configuration and may terminate if they are not valid.
/// The service will initialize and execute the processing pipeline constructed by the <see cref="IPipelineBuilder"/>
/// and terminating the application after the pipeline processing is complete.
/// </remarks>
public class ConsoleHostedService(
    IHostApplicationLifetime lifetime,
    ICommandLineConfig config,
    IPipelineBuilder pipelineBuilder,
    ILogger<ConsoleHostedService> logger
    ) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!config.IsValid || config is { InputFile: null })
        {
            logger.LogError("Command line arguments are not valid");
            Console.WriteLine("Command line arguments are not valid.");
            lifetime.StopApplication();
            return Task.CompletedTask;
        }

        logger.LogInformation("Processing input file {InputFile}", config.InputFile);

        var pipeline = pipelineBuilder.Build();
        pipeline.ProcessPipeline();

        logger.LogInformation("Pipeline processing completed successfully");
        lifetime.StopApplication();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}