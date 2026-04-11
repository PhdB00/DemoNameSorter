using DD.NameSorter;
using DD.NameSorter.Pipeline;
using NSubstitute;

namespace NameSorter.Tests.Pipeline;

[TestFixture]
public class PipelineProcessorTests
{
    private IPipelineStep extractStep;
    private IPipelineStep transformStep;

    [SetUp]
    public void Setup()
    {
        extractStep = Substitute.For<IPipelineStep, IPipelineExtractStep>();
        transformStep = Substitute.For<IPipelineStep, IPipelineTransformStep>();
    }
        
    [Test]
    public void ProcessPipeline_WithExtractStepOnly_CallsExtractStep()
    {
        // Arrange
        var steps = new List<IPipelineStep> { extractStep };
        var processor = new PipelineProcessor(steps);

        // Act
        processor.ProcessPipeline();
        
        // Assert
        ((IPipelineExtractStep)extractStep.Received(1)).Process();
    }
    
    [Test]
    public void ProcessPipeline_WithTransformStepOnly_CallsTransformStep()
    {
        // Arrange
        var steps = new List<IPipelineStep> { transformStep };
        var processor = new PipelineProcessor(steps);

        // Act
        processor.ProcessPipeline();
        
        // Assert
        ((IPipelineTransformStep)transformStep.Received(1)).Process(Arg.Any<List<Person>>());
    }
}