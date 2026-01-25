using System.ComponentModel;
using Perceptrons;

public class Trainer
{
    public Perceptron perceptron;

    public Trainer(int numOfInputs)
    {
        perceptron = new Perceptron(numOfInputs);
    }

    public void RandomMutation()
    {
        Random random = new Random();
        float rVal = random.NextSingle();
        rVal += random.Next(-2, 2);

        int mutatedVar = random.Next(-1, perceptron.weights.Length);
        if(mutatedVar < 0)
        {
            perceptron.bias += rVal;
            return;
        }

        perceptron.weights[mutatedVar] += rVal;
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

    public void Calculate(double[][] inputs, double[] targets, Func<double[], double[], double> ErrorCalc)
    {
        double[] oldWeights = new double[] {perceptron.weights[0], perceptron.weights[1]};
        double oldBias = perceptron.bias;
        double[] oldResults = perceptron.Compute(inputs);
        RandomMutation();

        double[] newResults = perceptron.Compute(inputs);
        if(ErrorCalc(newResults, targets) < ErrorCalc(oldResults, targets)) return;

        perceptron.weights = oldWeights;
        perceptron.bias = oldBias;
    }
}