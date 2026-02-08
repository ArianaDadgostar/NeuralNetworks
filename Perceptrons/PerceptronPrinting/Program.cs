using Perceptrons;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

double[][] inputs = new double[][]{ new double[]{0, 0}, new double[]{0, 1}, new double[]{1, 0}, new double[]{1, 1}};
double[] targets = new double[] {0, 1, 1, 1};


Trainer trainer = new Trainer(2);

while(true)
{ 
    Perceptron perceptron = new Perceptron(2);
    perceptron.Train(inputs, targets);
    //trainer.Calculate(inputs, targets, trainer.ErrorCalculation);
    double[] results = perceptron.ActivationCompute(inputs);
    for(int i = 0; i < perceptron.weights.Length; i++)
    {
        Console.WriteLine(perceptron.weights[i]);
    }
    Console.WriteLine(perceptron.bias); 
    //Console.WriteLine(trainer.ErrorCalculation(results, targets));
}


/*

    trainer.Calculate(inputs, targets, trainer.ErrorCalculation);
    //Console.WriteLine(trainer.perceptron.bias);
    // foreach(double weight in trainer.perceptron.weights)
    // {
    //     Console.WriteLine(weight);
    // }
    double[] results = trainer.perceptron.Compute(inputs);
    Console.WriteLine(trainer.perceptron.bias);
    foreach(double val in trainer.perceptron.weights)
    {
        Console.WriteLine(val);
    }
*/

// perceptron.weights[0]= 0.75;
// perceptron.weights[1] = -1.25;
// perceptron.bias = 0.5;

// double[] results = perceptron.Compute(inputs);

// foreach(double val in results)
// {
//     Console.WriteLine(val);
// }