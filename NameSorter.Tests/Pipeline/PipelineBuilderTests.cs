using System.Reflection;
using DD.NameSorter.Pipeline;

namespace NameSorter.Tests.Pipeline;

[TestFixture]
public class PipelineBuilderTests
{
    [Test]
    public void Build_WithValidSteps_ReturnsPipelineProcessor()
    {
        // Arrange
        var steps = new List<IPipelineStep>
        {
            new TestStep1(),
            new TestStep2()
        };
        var builder = new PipelineBuilder(steps);

        // Act
        var result = builder.Build();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<PipelineProcessor>());
    }

    [Test]
    public void Build_WithDuplicateStepOrders_ThrowsInvalidOperationException()
    {
        // Arrange
        var steps = new List<IPipelineStep>
        {
            new DuplicateOrderStep1(),
            new DuplicateOrderStep2()
        };
        var builder = new PipelineBuilder(steps);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Test]
    public void Build_OrdersStepsCorrectly()
    {
        // Arrange
        var step3 = new OrderedStep3();
        var step1 = new OrderedStep1();
        var step2 = new OrderedStep2();
            
        var steps = new List<IPipelineStep> { step3, step1, step2 };
        var builder = new PipelineBuilder(steps);

        // Act
        var result = builder.Build();

        // Assert
        var resultSteps = result.GetType()
            .GetField("<steps>P", BindingFlags.NonPublic | BindingFlags.Instance)?
            .GetValue(result) as IEnumerable<IPipelineStep>;
            
        Assert.Multiple(() =>
        {
            Assert.That(resultSteps, Is.Not.Null);
            var stepsList = resultSteps.ToList();
            Assert.That(stepsList, Has.Count.EqualTo(3));
            Assert.That(stepsList[0], Is.EqualTo(step1));
            Assert.That(stepsList[1], Is.EqualTo(step2));
            Assert.That(stepsList[2], Is.EqualTo(step3));
        });
    }

    [Test]
    public void Build_WithNoOrderAttribute_UsesMaxValue()
    {
        // Arrange
        var orderedStep = new OrderedStep1();
        var unorderedStep = new UnorderedStep();
            
        var steps = new List<IPipelineStep> { unorderedStep, orderedStep };
        var builder = new PipelineBuilder(steps);

        // Act
        var result = builder.Build();

        // Assert
        var resultSteps = result.GetType()
            .GetField("<steps>P", BindingFlags.NonPublic | BindingFlags.Instance)?
            .GetValue(result) as IEnumerable<IPipelineStep>;
            
        Assert.Multiple(() =>
        {
            Assert.That(resultSteps, Is.Not.Null);
            var stepsList = resultSteps.ToList();
            Assert.That(stepsList, Has.Count.EqualTo(2));
            Assert.That(stepsList[0], Is.EqualTo(orderedStep));
            Assert.That(stepsList[1], Is.EqualTo(unorderedStep));
        });
    }

    [Test]
    public void Build_WithNullSteps_CreatesEmptyPipeline()
    {
        // Arrange
        var steps = new List<IPipelineStep>();
        var builder = new PipelineBuilder(steps);

        // Act
        var result = builder.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            var resultSteps = result.GetType()
                .GetField("<steps>P", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(result) as IEnumerable<IPipelineStep>;

            Assert.That(resultSteps, Is.Not.Null);
            Assert.That(resultSteps, Is.Empty);
        });
    }
}
    
[PipelineStepOrder(1)]
public class OrderedStep1 : IPipelineStep;

[PipelineStepOrder(2)]
public class OrderedStep2 : IPipelineStep;

[PipelineStepOrder(3)]
public class OrderedStep3 : IPipelineStep;

public class UnorderedStep : IPipelineStep;

[PipelineStepOrder(1)]
public class DuplicateOrderStep1 : IPipelineStep;

[PipelineStepOrder(1)]
public class DuplicateOrderStep2 : IPipelineStep;

public class TestStep1 : IPipelineStep;

public class TestStep2 : IPipelineStep;