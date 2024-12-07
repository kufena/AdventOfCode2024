// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
long total = 0;
foreach (var line in lines)
{
    var splits = line.Split(':',StringSplitOptions.RemoveEmptyEntries);
    long result = long.Parse(splits[0]);
    var valsplits = splits[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    Stack<long> stack = new Stack<long>();
    stack.Push(0);
    for (int i = 0; i < valsplits.Length; i++)
    {
        long v = long.Parse(valsplits[i]);
        Stack<long> newstack = new Stack<long>();
        while (stack.Count > 0)
        {
            long c = stack.Pop();
            if (c + v <= result) newstack.Push(c + v);
            if (c * v <= result) newstack.Push(c * v);
        }
        stack = newstack;
    }
    if (stack.Count > 0)
    {
        if (stack.Contains(result))
        {
            Console.WriteLine(line);
            total += result;
        }
    }
}
Console.WriteLine($"{total}");