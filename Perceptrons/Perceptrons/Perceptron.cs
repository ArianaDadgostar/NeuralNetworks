namespace Perceptrons
{
    public class Perceptron
    {
        public double[] weights;
        public double bias;

        public Perceptron(int numOfInputs)
        {
            weights = new double[numOfInputs];
        }

        public Perceptron(int numOfInputs, double bias)
        {
            weights = new double[numOfInputs];
            this.bias = bias;
        }

        public bool Randomize(Random random, int min, int max, double[][] inputs, double goal)
        {
            double[] newWeights = new double[weights.Length];
            for (int i = 0; i < weights.Length; i++)
            {
                newWeights[i] = (max - min) * random.NextSingle() + min;
            }

            double newBias = (max - min) * random.NextSingle() + min;

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

        public bool Compare(double[][] inputs, double[] ogWeights, double[] newWeights, double ogBias, double newBias, double goal)
        {
            double originalResult = 0;
            double newResult = 0;
            
            foreach(double[] input in inputs)
            {
                double original = Compute(input, ogWeights, ogBias);
                double generated = Compute(input, newWeights, newBias);

                originalResult += Math.Abs(original - goal);
                newResult = Math.Abs(generated - goal);

            }
            
            if(originalResult < newResult) return false;

            weights = newWeights;
            bias = newBias;
            return true;
        }
    }
}