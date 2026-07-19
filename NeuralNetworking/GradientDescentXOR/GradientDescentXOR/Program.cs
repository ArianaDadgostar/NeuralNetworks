// See https://aka.ms/new-console-template for more information
using System;
namespace Program
{
    class Program
    {
        static double[] CalculateExpected(int first, int second)
        {
            return new double[] {first ^ second};
        }

        public static double ErrorCalculation(Network network, double[] desiredOutputs)
        {
            double error = 0;
            for(int i = 0; i < desiredOutputs.Length; i++)
            {
                error += network.outputs[i] - desiredOutputs[i];
                error = Math.Pow(error, 2);
            }
            return error;
        }

        public static double ErrorFuncDerivative(double output, double desiredOutput)
        {
            double error = output - desiredOutput;
            error *= 2;
            return error;
        }

        static void Setup(Network network)
        {
            network.layers = new Layer[3];
            int[] layerCount = new int[3] {2, 2, 1};
            ActivationFunc activationFunc = new(ActivationFunc.Function, ActivationFunc.Derivative);

            network.layers[0] = new Layer(activationFunc, layerCount[0], null);

            for(int i = 1; i < network.layers.Length; i++)
            {
                network.layers[i] = new Layer(activationFunc, layerCount[i], network.layers[i - 1]);
            }
            network.Randomize();
        }

        static double FilterError(Network network, double[][] inputs, double learningRate)
        {
            double error = 0;
            for(int i = 0; i < inputs.Length; i++)
            {
                network.Compute(inputs[i]);
                // int rounded = (network.outputs[0] > 0) ? 1 : 0;
                for(int j = 0; j < network.outputs.Length; j++)
                {
                    error += Math.Pow(network.outputs[j] - CalculateExpected((int)inputs[i][0], (int)inputs[i][1])[j], 2);
                }
                Console.WriteLine($"Input: {inputs[i][0]}, {inputs[i][1]} | Output: {network.outputs[0]} | Expected: {CalculateExpected((int)inputs[i][0], (int)inputs[i][1])[0]} | Error: {error}");
            }
            return error;
        }

        static void Main(string[] args)
        {
            Network network = new Network();
            Setup(network);
            
            const double learningRate = 0.01;

            double[][] inputs =
            {
                new double[2] {0, 0},
                new double[2] {0, 1},
                new double[2] {1, 0},
                new double[2] {1, 1}
            };

            double[][] desiredOutputs =
            {
                new double[1] {0},
                new double[1] {1},
                new double[1] {1},
                new double[1] {0}
            };
            
            while(Math.Abs(FilterError(network, inputs, learningRate)) > 0.1)
            {
                network.Train(inputs, desiredOutputs, learningRate);
            }
        }
    }
}