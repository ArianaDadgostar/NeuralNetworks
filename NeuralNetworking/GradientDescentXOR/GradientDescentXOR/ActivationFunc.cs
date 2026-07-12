namespace Program
{
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
            // double posE = Math.Pow(Math.E, input);
            // double negE = Math.Pow(Math.E, -1 * input);

            // return (posE - negE) / (negE + posE);
            return input;
        }

        public static double Nothing(double input)
        {
            return input;
        }

        public double Compute(double input)
        {
            return activation(input);
        }

        public static double Derivative(double input)
        {
            return 1 - Math.Pow(Function(input), 2);
        }
    }
}