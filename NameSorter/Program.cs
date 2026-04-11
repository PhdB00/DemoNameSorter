using DD.NameSorter;
using DD.NameSorter.Configuration;
using DD.NameSorter.Infrastructure;
using DD.NameSorter.Pipeline;
using DD.NameSorter.Pipeline.Output;
using DD.NameSorter.Pipeline.ReadNames;
using DD.NameSorter.Pipeline.SortNames;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

try
{
    var builder = CreateHostBuilder(args);
    builder = ConfigureServices(builder, args);

    await builder.RunConsoleAsync();

}
catch (Exception ex)
{
    Console.WriteLine("We're sorry, but an error occurred.");
    Console.WriteLine(ex.Message);
}

return;

static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .UseDefaultServiceProvider(options =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });
}

static IHostBuilder ConfigureServices(IHostBuilder builder, string[] args)
{
    return builder
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<ICommandLineConfig>(_ => new CommandLineConfig(args));
            services.AddTransient<IConsoleWriter, ConsoleWriter>();
            services.AddTransient<IFileSystem, FileSystem>();
            services.AddHostedService<ConsoleHostedService>();
            services.AddTransient<IOutputStrategy, ConsoleOutputStrategy>();
            services.AddTransient<IOutputStrategy, FileOutputStrategy>();
            services.AddTransient<IPipelineStep, ReadNamesExtractStep>();
            services.AddTransient<IPipelineStep, OutputNamesTransformStep>();
            services.AddTransient<IPipelineStep, SortNamesTransformStep>();
            services.AddTransient<PipelineProcessor, PipelineProcessor>();
            services.AddTransient<IPipelineBuilder, PipelineBuilder>();
            services.AddSingleton<INameParser, NameParser>(); 
            services.AddSingleton<INameSorter, NameSorter>();
        });
}