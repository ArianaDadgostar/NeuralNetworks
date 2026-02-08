using System.ComponentModel;
using Perceptrons;

/*
AND/OR Gate With Activation Function: Implement an activation function into your perceptron class.
Try out using different activation functions for your AND and OR gates.
*/

public class ActivationFunc
{
    public Func<double, double> activation;
    public Func<double, double> derivative;

    public ActivationFunc(Func<double, double> activation, Func<double, double> derivative)
    {
        this.activation = activation;
        this.derivative = derivative;
    }

    public static double Function(double input)
    {
        if(input > 0) return 1;
        return 0;
    }

    public static double Derivative()
    {
        return 1;
    }
}