namespace Perceptrons
{
    public class Perceptron
    {
        double[] weights;
        double bias;

        public Perceptron(int numOfInputs)
        {
            weights = new double[numOfInputs];
        }

        public Perceptron(int numOfInputs, double bias)
        {
            weights = new double[numOfInputs];
            this.bias = bias;
        }

        public bool Randomize(Random random, int min, int max, double[] inputs, double goal)
        {
            double[] newWeights = new double[weights.Length];
            for (int i = 0; i < weights.Length; i++)
            {
                newWeights[i] = random.Next(min, max);
            }

            double newBias = random.Next(min, max);

            return Compare(inputs, weights, newWeights, bias, newBias, goal);
        }

        public double Compute(double[] inputs, double[] newWeight, double newBias)
        {
            double result = 0;

            for(int i = 0; i < inputs.Length; i++)
            {
                result += (inputs[i] * newWeight[i]);
            }

            result += newBias;
            return result;
        }

        public bool Compare(double[] inputs, double[] ogWeights, double[] newWeights, double ogBias, double newBias, double goal)
        {
            double original = Compute(inputs, ogWeights, ogBias);
            double generated = Compute(inputs, newWeights, newBias);

            if(Math.Abs(original - goal) < Math.Abs(generated) - goal) return false;

            weights = newWeights;
            bias = newBias;
            
            return true;
        }
    }
}