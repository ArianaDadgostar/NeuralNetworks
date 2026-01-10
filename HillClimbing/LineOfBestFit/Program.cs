namespace LineOfBestFit
{
    internal class Program
    {
        static int CalculateFit(int[] points, int slope, int b)
        {
            int result = 0;
            for (int i = 0; i < points.Length; i++)
            {
                int line = (slope * points[i]) + b;
                result += points[i] - line;
            }

            return result / points.Length;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
