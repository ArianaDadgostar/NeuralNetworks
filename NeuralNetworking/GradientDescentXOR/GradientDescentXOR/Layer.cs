namespace Program
{
    public class Layer
    {
        public Neuron[] neurons;
        public double[] outputs;

        public Layer(ActivationFunc activation, int neuronCount, Layer previousLayer)
        {
            neurons = new Neuron[neuronCount];
            outputs = new double[neuronCount];
            for(int i = 0; i < neuronCount; i++)
            {
                neurons[i] = (previousLayer == null) ? new Neuron(activation, new Neuron[0]) : new Neuron(activation, previousLayer.neurons);
                neurons[i].Activation = activation;
                for(int j = 0; j < neurons[i].dendrites.Length; j++)
                {
                    neurons[i].dendrites[j] = new Dendrite(previousLayer.neurons[j], neurons[i], 0);
                }
            }
        }

        public void ApplyUpdates()
        {
            foreach(Neuron neuron in neurons)
            {
                neuron.ApplyUpdates();
            }
        }

        public void Backpropogate(double learningRate)
        {
            foreach(Neuron neuron in neurons)
            {
                neuron.Backpropogate(learningRate);
            }
        }

        public void Calculate()
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
}