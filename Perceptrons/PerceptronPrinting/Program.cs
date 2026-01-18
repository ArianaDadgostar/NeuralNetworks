using Perceptrons;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


Perceptron perceptron = new Perceptron(inputs.Length);

Random random = new Random();
//while(!perceptron.Randomize(random, 1, 100, inputs, goal[0]));