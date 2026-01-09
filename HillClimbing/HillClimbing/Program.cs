// See https://aka.ms/new-console-template for more information

class Program 
{
    public const int MINCHAR = 32;
    public const int MAXCHAR = 126;

    static string RandomMutate(string current)
    {
        var random = new Random();
        int index = random.Next(0, current.Length);

        int change = -100000;
        while(change + current[index] >= MINCHAR && change + current[index] <= MAXCHAR)
        {
            change = random.Next(1, 3);
            if(change > 1)
            {
                change = -1;
                continue;
            }
        }
        string newString = "";
        
        for(int i = 0; i < current.Length; i++)
        {
            if(i == index)
            {
                newString += (char)(current[i] + change);
                continue;
            }
            newString += current[i];
        }
        return newString;
    }
    static int Compare(string current, string goal)
    {
        int result = 0;
        for (int i = 0; i < goal.Length; i++)
        {
            result += (goal[i] - current[i]);
        }
        return result/goal.Length;
    }

    static void Main()
    {
        Console.WriteLine("Insert word to uncover: ");
        string goal = Console.ReadLine();
        string randomString = Guid.NewGuid().ToString();

        while(true)
        {
            string newString = RandomMutate(randomString);
            if(Compare(goal, newString) < Compare(goal, randomString))
            {
                randomString = newString;
                Console.Clear();
                Console.WriteLine(randomString);
            }
        }
    }
}