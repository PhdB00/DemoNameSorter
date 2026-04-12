using BenchmarkDotNet.Attributes;
using DD.NameSorter.Pipeline.ReadNames;
using Microsoft.Extensions.Logging.Abstractions;

namespace NameSorter.Benchmarks.Pipeline.ReadNames;

[MemoryDiagnoser]
[CategoriesColumn]
[AllStatisticsColumn]
public class NameParserBenchmarks
{
    private INameParser parser;
    private string[] twoPartNames;
    private string[] threePartNames;
    private string[] fourPartNames;

    [Params(100, 1000, 10000)]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int NameCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        parser = new NameParser(NullLogger<NameParser>.Instance);
        
        twoPartNames = GenerateNames(NameCount, 2);
        threePartNames = GenerateNames(NameCount, 3);
        fourPartNames = GenerateNames(NameCount, 4);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("TwoParts")]
    public void ParseTwoPartNames()
    {
        foreach (var name in twoPartNames)
        {
            parser.ParseName(name);
        }
    }

    [Benchmark]
    [BenchmarkCategory("ThreeParts")]
    public void ParseThreePartNames()
    {
        foreach (var name in threePartNames)
        {
            parser.ParseName(name);
        }
    }

    [Benchmark]
    [BenchmarkCategory("FourParts")]
    public void ParseFourPartNames()
    {
        foreach (var name in fourPartNames)
        {
            parser.ParseName(name);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Mixed")]
    public void ParseMixedNames()
    {
        for (int i = 0; i < NameCount; i++)
        {
            parser.ParseName(twoPartNames[i % twoPartNames.Length]);
            parser.ParseName(threePartNames[i % threePartNames.Length]);
            parser.ParseName(fourPartNames[i % fourPartNames.Length]);
        }
    }

    [Benchmark]
    [BenchmarkCategory("EdgeCases")]
    public void ParseEdgeCases()
    {
        for (int i = 0; i < NameCount; i++)
        {
            try
            {
                // Test with extra spaces
                parser.ParseName("  John    Doe  ");
                
                // Test with maximum allowed parts
                parser.ParseName("John James Robert Smith");
            }
            catch
            {
                // Ignore exceptions in benchmark
            }
        }
    }

    private static string[] GenerateNames(int count, int parts)
    {
        var random = new Random(42); // Fixed seed for reproducibility
        var names = new string[count];
        var firstNames = new[] { "John", "Jane", "Robert", "Mary", "James", "Patricia", "Michael", "Linda" };
        var middleNames = new[] { "Alan", "Marie", "Lee", "Ann", "Peter", "Rose", "William", "Elizabeth" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis" };

        for (int i = 0; i < count; i++)
        {
            var nameBuilder = new List<string>();
            
            // Add first name
            nameBuilder.Add(firstNames[random.Next(firstNames.Length)]);
            
            // Add middle names if needed
            for (int j = 0; j < parts - 2; j++)
            {
                nameBuilder.Add(middleNames[random.Next(middleNames.Length)]);
            }
            
            // Add last name
            nameBuilder.Add(lastNames[random.Next(lastNames.Length)]);
            
            names[i] = string.Join(" ", nameBuilder);
        }

        return names;
    }
}