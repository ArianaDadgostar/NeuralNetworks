using System.Net.Sockets;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using FeedForwardNetwork;

public class SinProj
{
    Network[] networks;

    public SinProj(int length)
    {
        networks = new Network[length];
        for(int i = 0; i < networks.Length; i ++)
        {
            networks[i] = new Network();
            networks[i].layers = new Layer[3];
            for(int j = 0; j < networks[i].layers.Length; j++)
            {
                ActivationFunc acti = new ActivationFunc(ActivationFunc.SinFunc, ActivationFunc.Derivative);
                networks[i].layers[j] = new Layer(acti, 100);
                //(-1 * Math.Abs(j - 1)) + 2
            }
        }
    }

    public double FitnessFunc(double[] inputs, Network network)
    {
        for(int i = 0; i < network.layers.Length; i++)
        {
            network.layers[i].Calculate(inputs);
            //inputs = network.layers[i].outputs;
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
        for(int j = 0; j < networks.Length; j++)
        {
            for(int i = 0; i < inputs.Length; i++)
            {
                foreach(Layer layer in networks[j].layers)
                {
                    networks[j].layers[0].Calculate(new double[] { inputs[i] });
                }
            }
            population.Enqueue(networks[j], FitnessFunc(inputs, networks[j]));
        }

        return population;
    }

    public void Mutate(double[] inputs)
    {
        PriorityQueue<Network, double> population = Sort(inputs);
        Network[] top = new Network[(int)(population.Count * 0.1)];
        Network[] bottom = new Network[top.Length];
        Network[] middle = new Network[population.Count - (top.Length + bottom.Length)];

        int popCount = population.Count;

        for(int i = 0; i < popCount; i ++)
        {
            if(i < top.Length)
            {
                top[i] = population.Dequeue();
                continue;
            }
            else if(i < middle.Length + top.Length)
            {
                middle[i - top.Length] = population.Dequeue();
                continue;
            }

            bottom[i-(top.Length + middle.Length)] = population.Dequeue();
        }

        CrossOver(top, middle);
        for(int i = 0; i < bottom.Length; i++)
        {
            bottom[i].Randomize();
        }
    }

    public void CrossOver(Network[] top, Network[] middle)
    {
        Random random = new Random();
        for(int w = 0; w < top.Length; w++)
        {
            for(int l = 0; l < top[w].layers.Length; l ++)
            {
                for(int n = 0; n < top[w].layers[l].neurons.Length; n ++)
                {
                    double split = random.Next(top[w].layers[l].neurons[n].dendrites.Length);
    
                    for(int d = 0; d < top[w].layers[l].neurons[n].dendrites.Length; d++)
                    {
                        if(d < split)
                        {
                            middle[w].layers[l].neurons[n].dendrites[d].weight = top[w].layers[l].neurons[n].dendrites[d].weight;
                        }
                    }
                }
            }
        }
    }
    
    public double Train(double[] inputs)
    {

        Mutate(inputs);
        double averageFitness = 0;
        foreach(Network network in networks)
        {
            averageFitness += FitnessFunc(inputs, network);
        }
        return averageFitness / networks.Length;
    }
}