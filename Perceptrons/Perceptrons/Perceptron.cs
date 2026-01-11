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

        public void Randomize(Random random, int min, int max)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = random.Next(min, max);
            }

            bias = random.Next(min, max);
        }
    }
}