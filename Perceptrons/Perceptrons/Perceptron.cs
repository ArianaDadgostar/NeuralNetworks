using System.Globalization;

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

        public void Randomize(Random random, int min, int max)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = (max - min) * random.NextSingle() + min;
            }

            bias = (max - min) * random.NextSingle() + min;

            //return Compare(inputs, weights, newWeights, bias, newBias, goal);
        }

        public double Compute(double[] inputs)
        {
            double result = 0;

            for(int i = 0; i < inputs.Length; i++)
            {
                result += (inputs[i] * weights[i]);
            }

            result += bias;
            return result;
        }


        public double[] Compute(double[][] inputs)
        {
            double[] result = new double[inputs.Length];
            for(int j = 0; j < result.Length; j++)
            {
                result[j] = Compute(inputs[j]);
            }
            return result;
        }

        // public bool Compare(double[][] inputs, double[] ogWeights, double[] newWeights, double ogBias, double newBias, double goal)
        // {
        //     double originalResult = 0;
        //     double newResult = 0;
            
        //     foreach(double[] input in inputs)
        //     {
        //         double original = Compute(input, ogWeights, ogBias);
        //         double generated = Compute(input, newWeights, newBias);

        //         originalResult += Math.Abs(original - goal);
        //         newResult = Math.Abs(generated - goal);

        //     }
            
        //     if(originalResult < newResult) return false;

        //     weights = newWeights;
        //     bias = newBias;
        //     return true;
        // }
    }
}