namespace FeedForwardNetwork.Tests;
using FeedForwardNetwork;
using Xunit.Abstractions;

public class FeedForwardNetworkTests
{
    private readonly ITestOutputHelper output;

    // Constructor to inject ITestOutputHelper
    public FeedForwardNetworkTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void FeedForward_WithKnownWeights_ReturnsCorrectOutput()
    {
        // Identity activation for testing
        ActivationFunc identity = new ActivationFunc(x => x, x => 1.0);

        // Create neurons
        Neuron input1 = new Neuron();
        Neuron input2 = new Neuron();
        Neuron outputNeuron = new Neuron();
        outputNeuron.Activation = identity;

        // Set fixed outputs for input neurons
        typeof(Neuron).GetProperty("Output")!.SetValue(input1, 1.0);
        typeof(Neuron).GetProperty("Output")!.SetValue(input2, 2.0);

        // Create dendrites (connections)
        Dendrite d1 = new Dendrite(input1, outputNeuron, 0.5);
        Dendrite d2 = new Dendrite(input2, outputNeuron, -1.0);

        outputNeuron.dendrites = new[] { d1, d2 };

        // ---- Feedforward calculation manually ----
        double sum = 0;
        foreach (var d in outputNeuron.dendrites)
            sum += d.previous.Output * d.weight;

        double output = outputNeuron.Activation.Compute(sum);

        // Expected:
        // 1 * 0.5 + 2 * (-1.0) = -1.5
        double expected = -1.5;

        Assert.Equal(expected, output);
    }

    [Fact]

    public void SinProj_FitnessFunc_ReturnsExpectedFitness()
    {
        SinProj sinProj = new SinProj(100);

        
        Random random = new Random();
        double[] inputs = new double[100];
        for(int i = 0; i < inputs.Length; i ++)
        {  
            inputs[i] = (double)random.NextDouble() * random.Next(-1, 1);
        }
            double result = sinProj.Train(inputs );

        while(Math.Abs(result) > 0.01) // Arbitrary threshold for "good enough" fitness
        {
            result = sinProj.Train(inputs);
            // The expected output for sin(pi/2) is 1, so fitness should be close to 0
            output.WriteLine("Fitness: " + result);
        }
        ;
    }
}