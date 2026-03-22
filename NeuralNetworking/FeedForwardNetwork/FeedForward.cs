namespace FeedForwardNetwork;

public class Dendrite
{
    public Neuron previous { get; }
    public Neuron next { get; }
    public double weight { get; set; }

    public Dendrite(Neuron previous, Neuron next, double weight)
    {
        this.previous = previous;
        this.next = next;
        this.weight = weight;
    }

    public double Compute()
    {
        return previous.Output * weight;
    }
}

public class Neuron
{
    double bias;
    public Dendrite[] dendrites;
    public double Input { get; }
    public double Output { get; private set; }
    public ActivationFunc Activation ;

    public void Compute()
    {
        double sum = bias;
        foreach (var d in dendrites)
            sum += d.Compute();

        Output = Activation.Compute(sum);
    }


    public void Randomize(Random random)
    {
        for(int i = 0; i < dendrites.Length; i++)
        {
            dendrites[i].weight = random.Next(0, 100);
        }
        bias = random.Next(-1, 1);
    }
}

public class Layer
{
    public Neuron[] neurons;
    public double[] outputs;

    public Layer(ActivationFunc activation, int neuronCount)
    {
        neurons = new Neuron[neuronCount];
        outputs = new double[neuronCount];
        for(int i = 0; i < neuronCount; i++)
        {
            neurons[i] = new Neuron();
            neurons[i].Activation = activation;
            neurons[i].dendrites = new Dendrite[5];
            for(int j = 0; j < neurons[i].dendrites.Length; j++)
            {
                neurons[i].dendrites[j] = new Dendrite(new Neuron(), neurons[i], 0);
            }
        }
    }

    public void Calculate(double[] inputs)
    {
        for(int i = 0; i < neurons.Length; i ++)
        {
            neurons[i].Compute();
            outputs[i] = neurons[i].Output;
        }
    }

    public void Randomize(Random random)
    {
        for(int i = 0; i < neurons.Length; i++)
        {
            neurons[i].Randomize(random);
        }
    }
}

public class Network
{
    public Layer[] layers;
    public double[] outputs;
    public Action<int> ErrorCalculation;

    public void Randomize()
    {
        Random random = new Random();
        for(int i = 0; i < layers.Length; i ++)
        {
            layers[i].Randomize(random);
        }
    }
}