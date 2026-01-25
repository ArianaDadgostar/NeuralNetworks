using Perceptrons;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

double[][] inputs = new double[][]{ new double[]{0, 0}, new double[]{0.3, -0.7}, new double[]{1, 1}, new double[]{-1, -1}, new double[]{-0.5, 0.5}};

Perceptron perceptron = new Perceptron(inputs.Length);

perceptron.weights[0]= 0.75;
perceptron.weights[1] = -1.25;
perceptron.bias = 0.5;

double[] results = perceptron.Compute(inputs);

foreach(double val in results)
{
    Console.WriteLine(val);
}