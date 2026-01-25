using Perceptrons;

public class Trainer
{
    Perceptron perceptron;

    public Trainer(int numOfInputs)
    {
        perceptron = new Perceptron(numOfInputs);
    }

    public double ErrorCalculation(double[] results, double[] targets)
    {
        double error = 0;
        for(int i = 0; i < results.Length; i ++)
        {
            error += (targets[i] - results[i]) * (targets[i] - results[i]);
        }
        error /= results.Length;
        return error;
    }

    public void Calculate(int[][] inputs, double[] targets, )
}