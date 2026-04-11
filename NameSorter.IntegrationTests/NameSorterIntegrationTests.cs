using DD.NameSorter.Configuration;
using DD.NameSorter.Infrastructure;
using DD.NameSorter.Pipeline;
using DD.NameSorter.Pipeline.Output;
using DD.NameSorter.Pipeline.ReadNames;
using DD.NameSorter.Pipeline.SortNames;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace NameSorter.IntegrationTests;

[TestFixture]
public class NameSorterIntegrationTests
{
    private const string TestInputFile = "test-names.txt";
    private const string TestOutputFile = "test-sorted-names.txt";
    
    private IServiceProvider? serviceProvider;
    private List<string> consoleOutput = null!;
    private IFileSystem fileSystem = null!;
    private ICommandLineConfig config = null!;
    private IConsoleWriter consoleWriter = null!;

    [SetUp]
    public void Setup()
    {
        consoleOutput = [];
        SetupMocks();
        ConfigureServices();
    }

    [TearDown]
    public void TearDown()
    {
        if (serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private void SetupMocks()
    {
        // Setup Config
        config = Substitute.For<ICommandLineConfig>();
        config.InputFile.Returns(TestInputFile);
        config.OutputFile.Returns(TestOutputFile);
        config.IsValid.Returns(true);

        // Setup FileSystem
        fileSystem = Substitute.For<IFileSystem>();
        fileSystem.Exists(TestInputFile).Returns(true);

        // Setup Console Writer
        consoleWriter = Substitute.For<IConsoleWriter>();
        consoleWriter
            .When(x => x.WriteLine(Arg.Any<string>()))
            .Do(x => consoleOutput.Add(x.Arg<string>()));
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        
        services.AddSingleton(config);
        services.AddSingleton(fileSystem);
        services.AddSingleton(consoleWriter);
        
        services.AddTransient<IOutputStrategy, ConsoleOutputStrategy>();
        services.AddTransient<IOutputStrategy, FileOutputStrategy>();
        services.AddTransient<IPipelineStep, ReadNamesExtractStep>();
        services.AddTransient<IPipelineStep, OutputNamesTransformStep>();
        services.AddTransient<IPipelineStep, SortNamesTransformStep>();
        services.AddTransient<IPipelineBuilder, PipelineBuilder>();
        services.AddTransient<INameParser, NameParser>();
        services.AddTransient<INameSorter, DD.NameSorter.Pipeline.SortNames.NameSorter>();

        serviceProvider = services.BuildServiceProvider();
    }

    [Test]
    public void ProcessPipeline_WithValidInput_SortsAndOutputsCorrectly()
    {
        // Arrange
        var inputNames = new[]
        {
            "Janet Parsons",
            "Vaughn Lewis",
            "Adonis Julius Archer",
            "Shelby Nathan Yoder"
        };
        
        var expectedSortedNames = new[]
        {
            "Adonis Julius Archer",
            "Vaughn Lewis",
            "Janet Parsons",
            "Shelby Nathan Yoder"
        };

        fileSystem.ReadAllLines(TestInputFile).Returns(inputNames);

        // Act
        var pipeline = serviceProvider!.GetRequiredService<IPipelineBuilder>().Build();
        pipeline.ProcessPipeline();

        // Assert
        fileSystem.Received(1).WriteAllLines(
            TestOutputFile, 
            Arg.Is<IEnumerable<string>>(x => x.SequenceEqual(expectedSortedNames)));
        
        Assert.That(consoleOutput, Is.EqualTo(expectedSortedNames));
    }

    [Test]
    public void ProcessPipeline_WithEmptyInput_HandlesGracefully()
    {
        // Arrange
        fileSystem.ReadAllLines(TestInputFile).Returns([]);

        // Act
        var pipeline = serviceProvider!.GetRequiredService<IPipelineBuilder>().Build();
        pipeline.ProcessPipeline();

        // Assert
        Assert.That(consoleOutput, Is.Empty);
        fileSystem.DidNotReceive().WriteAllLines(
            Arg.Any<string>(), 
            Arg.Any<IEnumerable<string>>());
    }

    [TestCase<string>("SingleName")]
    [TestCase<string>("Aaron Beavis Colin David Edward Frank George Henry Hastoomanynames")]
    public void ProcessPipeline_WithInvalidNames_ThrowsArgumentException(string name)
    {
        // Arrange
        var inputNames = new[] { name };
        
        fileSystem.ReadAllLines(TestInputFile).Returns(inputNames);

        // Act, Assert
        var pipeline = serviceProvider!.GetRequiredService<IPipelineBuilder>().Build();
        Assert.That(() => pipeline.ProcessPipeline(), 
            Throws.ArgumentException);
    }

    [Test]
    public void ProcessPipeline_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        fileSystem.Exists(TestInputFile).Returns(false);

        // Act, Assert
        var pipeline = serviceProvider!.GetRequiredService<IPipelineBuilder>().Build();
        Assert.That(() => pipeline.ProcessPipeline(), 
            Throws.TypeOf<FileNotFoundException>());
    }
}