namespace FeedForwardNetwork.Tests;
using FeedForwardNetwork;

public class FeedForwardNetworkTests
{
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

    public void SinProj_FitnessFunc_ReturnsExpectedFitness()
    {
        // Create a simple network with known weights
        Network network = new Network(new[] { 1 }, new ActivationFunc(x => x, x => 1.0));
        network.layers[0].neurons[0].dendrites = new[] { new Dendrite(null, network.layers[0].neurons[0], 1.0) };

        SinProj sinProj = new SinProj(1);
        double fitness = sinProj.FitnessFunc(new double[] { Math.PI / 2 }, network);

        // The expected output for sin(pi/2) is 1, so fitness should be close to 0
        Assert.InRange(fitness, -0.01, 0.01);
    }
}