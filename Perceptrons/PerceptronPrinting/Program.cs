using Perceptrons;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

double[][] inputs =
{
    new double[] {1, 1},
    new double[] { 1, 0},
    new double[] {0, 1},
    new double[] {0, 0}
};

double[] goal = {1, 0, 0, 0};

Perceptron perceptron = new Perceptron(inputs.Length);

Random random = new Random();
while(!perceptron.Randomize(random, 1, 100, inputs, goal[0]))
{
    foreach(double weight in perceptron.weights)
    {
        Console.WriteLine(weight);
    }
    Console.Write(perceptron.bias);
}