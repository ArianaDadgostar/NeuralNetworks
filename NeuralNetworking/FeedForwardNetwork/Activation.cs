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

    public double Compute(double input)
    {
        return activation(input);
    }

    public static double Derivative()
    {
        return 1;
    }
}