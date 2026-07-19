using System.Runtime.CompilerServices;

namespace Program
{
    public class Network
    {
        public Layer[] layers;
        public double[] outputs;
        public Action<int> ErrorCalculation;

        public void ApplyUpdates()
        {
            foreach(Layer layer in layers)
            {
                layer.ApplyUpdates();
            }
        }

        public void Backpropogate(double learningRate, double[] desiredOutputs)
        {
            for(int i = 0; i < layers[^1].neurons.Length; i++)
            {
                Neuron neuron = layers[^1].neurons[i];
                double input = neuron.ComputeNoActivation();
                neuron.Delta = Program.ErrorFuncDerivative(neuron.Output, desiredOutputs[i]);
                neuron.BiasUpdate = neuron.Delta * neuron.Activation.derivative(input) * learningRate;
                foreach(Dendrite dendrite in neuron.dendrites)
                {
                    Neuron previous = dendrite.previous;
                    if(previous is null) continue;

                    previous.Delta += neuron.Delta * neuron.Activation.derivative(input) * dendrite.weight;
                    dendrite.weightUpdate += neuron.Delta * neuron.Activation.derivative(input) * previous.Output * learningRate;
                }

                neuron.Delta = 0;
            }
            for(int i = layers.Length - 2; i >= 0; i--)
            {
                layers[i].Backpropogate(learningRate);
            }
        }

        public double Train(double[][] inputs, double[][] desiredOutputs, double learningRate)
        {
            double error = 0;
            for(int i = 0; i < inputs.Length; i ++)
            {
                Compute(inputs[i]);
                Backpropogate(learningRate, desiredOutputs[i]);
                ApplyUpdates();
                error += Program.ErrorCalculation(this, desiredOutputs[i]);
            }
            return error / inputs.Length;
        }

        public void Randomize()
        {
            Random random = new Random();
            for(int i = 0; i < layers.Length; i ++)
            {
                layers[i].Randomize(random);
            }
        }

        public void Compute(double[] inputs) 
        {
            layers[0].outputs = inputs;

            for(int i = 0; i < layers[0].neurons.Length; i++)
            {
                layers[0].neurons[i].Output = inputs[i];
            }
            
            for(int i = 1; i < layers.Length; i++)
            {
                layers[i].Calculate();
            }
            outputs = layers[^1].outputs;
        }
    }
}