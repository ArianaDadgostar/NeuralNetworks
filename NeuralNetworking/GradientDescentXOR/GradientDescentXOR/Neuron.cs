namespace Program
{
    public class Neuron
    {
        double bias;
        public Dendrite[] dendrites;
        public double Input { get; }
        public double Output { get; set; }
        public ActivationFunc Activation;
        public double Delta { get; set; }
        public double BiasUpdate { get; set; }

        public Neuron(ActivationFunc activation, Neuron[] previousNeurons)
        {
            Activation = activation;
            dendrites = new Dendrite[previousNeurons.Length];
            for(int i = 0; i < dendrites.Length; i ++)
            {
                dendrites[i] = new Dendrite(previousNeurons[i], this, 0);
            }
        }

        public void ApplyUpdates()
        {
            foreach(Dendrite dendrite in dendrites)
            {
                dendrite.ApplyUpdates();
            }
            bias -= BiasUpdate;
            BiasUpdate = 0;
        }

        public void Backpropogate(double learningRate)
        {
            double input = ComputeNoActivation();
            foreach(Dendrite dendrite in dendrites)
            {
                Neuron previous = dendrite.previous;
                if(previous == null) continue;

                previous.Delta += Delta * Activation.derivative(input) * dendrite.weight;
                dendrite.weightUpdate += Delta * Activation.derivative(input) * previous.Output * learningRate;
            }
            BiasUpdate = Delta * Activation.derivative(input) * learningRate;

            Delta = 0;
        }

        public double ComputeNoActivation()
        {
            double sum = bias;
            foreach (var d in dendrites)
                sum += d.Compute();
            return sum;
        }

        public double Compute()
        {
            double sum = bias;
            foreach (var d in dendrites)
                sum += d.Compute();
            Output = Activation.Compute(sum);
            return Output;
        }
    

        public void Randomize(Random random)
        {
            for(int i = 0; i < dendrites.Length; i++)
            {
                dendrites[i].weight = random.NextDouble();
            }
            bias = random.Next(0, 1) == 0 ? random.NextDouble() : random.NextDouble() * -1;
        }
    }
}