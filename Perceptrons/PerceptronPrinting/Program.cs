using Perceptrons;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

double[][] inputs = new double[][]{ new double[]{0, 0}, new double[]{0, 1}, new double[]{1, 0}, new double[]{1, 1}};
double[] targets = new double[] {0, 0, 0, 1};


Trainer trainer = new Trainer(2);

while(true)
{
    trainer.Calculate(inputs, targets, trainer.ErrorCalculation);
    //Console.WriteLine(trainer.perceptron.bias);
    // foreach(double weight in trainer.perceptron.weights)
    // {
    //     Console.WriteLine(weight);
    // }
    double[] results = trainer.perceptron.Compute(inputs);
    Console.WriteLine(trainer.ErrorCalculation(results, targets));
}

// perceptron.weights[0]= 0.75;
// perceptron.weights[1] = -1.25;
// perceptron.bias = 0.5;

// double[] results = perceptron.Compute(inputs);

// foreach(double val in results)
// {
//     Console.WriteLine(val);
// }