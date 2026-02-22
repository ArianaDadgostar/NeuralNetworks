using System.Net.Sockets;
using FeedForwardNetwork;

public class SinProj
{
    Network[] networks;

    public SinProj(int length)
    {
        networks = new Network[length];
    }

    public double FitnessFunc(double[] inputs, Network network)
    {
        for(int i = 0; i < network.layers.Length; i++)
        {
            network.layers[i].Calculate(inputs);
            inputs = network.layers[i].outputs;
        }
        network.outputs = network.layers[network.layers.Length - 1].outputs;

        double average = 0;

        for(int i = 0; i < network.outputs.Length; i ++)
        {
            double expected = Math.Sin(inputs[i]);
            average += expected - network.outputs[i];
        }
        return average/network.outputs.Length;
    }

    public PriorityQueue<Network, double> Sort(double[] inputs)
    {
        PriorityQueue<Network, double> population = new PriorityQueue<Network, double>();
        for(int i = 0; i < inputs.Length; i++)
        {
            networks[i].layers[0].Calculate(new double[] { inputs[i] });
            population.Enqueue(networks[i], FitnessFunc(inputs, networks[i]));
        }

        return population;
    }

    public void Mutate(double[] inputs)
    {
        PriorityQueue<Network, double> population = Sort(inputs);
    }
    
    public void Train()
    {
        
    }
}