using System.Globalization;
using System.Reflection.Metadata.Ecma335;

namespace Perceptrons
{
    public class Perceptron
    {
        public double[] weights;
        public double bias;

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
                weights[i] = (max - min) * random.NextSingle() + min;
            }

            bias = (max - min) * random.NextSingle() + min;

            //return Compare(inputs, weights, newWeights, bias, newBias, goal);
        }

        public double Compute(double[] inputs)
        {
            double result = 0;

            for(int i = 0; i < inputs.Length; i++)
            {
                result += (inputs[i] * weights[i]);
            }

            result += bias;
            return result;
        }

        public double[] RegularCompute(double[][] inputs)
        {
            double[] result = new double[inputs.Length];
            for(int j = 0; j < result.Length; j++)
            {
                result[j] = Compute(inputs[j]);
            }
            return result;
        }

#region Activation

        public double[] ActivationCompute(double[][] inputs)
        {
            double[] result = new double[inputs.Length];
            for(int j = 0; j < result.Length; j++)
            {
                result[j] = Compute(inputs[j]);
                result[j] = ActivationFunc.Function(result[j]);
            }
            return result;
        }

        public double ChangeCalculation(double[] inputs, double desiredOutput, double input)
        {
            double derivative = 0;
            foreach(double val in inputs)
            {
                derivative += 2 * (desiredOutput - val);
            }
            derivative /= inputs.Length;

            return derivative * ActivationFunc.Derivative();
        }

        public double Train(double[] inputs, double desiredOutput)
        {
            double output = Compute(inputs);
            double error = desiredOutput - output;

            for(int i = 0; i < weights.Length; i++)
            {
                weights[i] -= ChangeCalculation(inputs, desiredOutput, inputs[i]);
            }

            bias -= ChangeCalculation(inputs, desiredOutput, 1);

            return error * error;
        }

        public void ChangeValues(double[][] inputs, double[] desiredOutput)
        {
                for(int i = 0; i < inputs.Length; i++)
                {
                    for(int j = 0; j < weights.Length; j++)
                    {
                        double original = Trainer.StaticErrorCalculation(ActivationCompute(inputs), desiredOutput);
                        double changeValue = ChangeCalculation(inputs[i], desiredOutput[i], inputs[i][j]);
                        weights[j] += changeValue;
                        if(Trainer.StaticErrorCalculation(ActivationCompute(inputs), desiredOutput) < original) return;

                        weights[j] -= changeValue;
                    }

                        double originalBias = Trainer.StaticErrorCalculation(ActivationCompute(inputs), desiredOutput);
                        double biasChange = ChangeCalculation(inputs[i], desiredOutput[i], 1);
                        bias += biasChange;
                        if(Trainer.StaticErrorCalculation(ActivationCompute(inputs), desiredOutput) < originalBias) return;

                        bias -= biasChange;
                }
        }

        public double[] Train(double[][] inputs, double[] desiredOutput)
        {
            double[] errors = new double[inputs.Length];
            for(int i = 0; i < inputs.Length; i++)
            {
                double output = Compute(inputs[i]);
                output = ActivationFunc.Function(output);
                double error = desiredOutput[i] - output;

                ChangeValues(inputs, desiredOutput);

                errors[i] = error * error;
            }
            return errors;
        }
    }

#endregion
}