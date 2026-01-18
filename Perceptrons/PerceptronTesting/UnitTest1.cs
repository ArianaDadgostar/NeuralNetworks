using System.ComponentModel;
using Perceptrons;

namespace PerceptronTesting;

public class PerceptronTests
{
    public static readonly TheoryData<(double, double)[], int[], double[]> CompareTestData = new()
    {
        {
            new (double, double)[] { (1, 1), (0, 1), (1, 0), (0, 0) },
            new int[] { 1, 0 },
            new double[] { 0.5, 0 }
        }
    };

    [Theory]
    [MemberData(nameof(CompareTestData))]
    public void CompareTest((double, double)[] inputs, int[] goal, double[] answer)
    {
        Perceptron perceptron = new Perceptron(inputs.Length);

        Random random = new Random();
        //while(!perceptron.Randomize(random, 1, 100, inputs, goal[0]));

        
    }
}