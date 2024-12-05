// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

static bool TestTest(Dictionary<int, List<int>> rules, List<int> test, out int badkey, out int fury)
{

    bool[] seen = new bool[100];
    for (int i = 0; i < 100; i++) seen[i] = false;
    bool include = true;
    fury = -1;
    badkey = -1;

    foreach (int k in test)
    {
        bool ok = true;

        if (rules.ContainsKey(k))
        {
            foreach (int b in rules[k])
            {
                if (seen[b])
                {
                    ok = false;
                    Console.WriteLine($"{k} before {b} is a no no.");
                    badkey = k;
                    fury = b;
                    break;
                }
            }
        }
        if (!ok)
        {

            Console.WriteLine($"Fails on {k}");
            include = false;
            break;
        }
        seen[k] = true;
    }

    return include;
}

Dictionary<int, List<int>> rules = new Dictionary<int, List<int>>();

var lines = File.ReadAllLines(args[0]);
var tests = new List<List<int>>();

foreach (var line in lines)
{
    if (line.Contains('|'))
    {
        var splits = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
        int l1 = int.Parse(splits[0]);
        int l2 = int.Parse(splits[1]);
        if (rules.ContainsKey(l1))
            rules[l1].Add(l2);
        else
        {
            rules.Add(l1, new List<int>() { l2 });
        }
    }
    else if (!line.Trim().Equals(""))
    {
        var l = new List<int>();
        var lsp = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
        for (int j = 0; j < lsp.Length; j++)
            l.Add(int.Parse(lsp[j]));
        tests.Add(l);
    }
}
int count;

long total;

//Part1(rules, tests, out count, out total);
Part2(rules, tests, out count, out total);

Console.WriteLine($"{count} tests ok = total is {total}");

static void Part2(Dictionary<int, List<int>> rules, List<List<int>> tests, out int count, out long total)
{
    count = 0;
    total = 0;
    foreach (var test in tests)
    {
        foreach (var k in test)
        {
            Console.Write($"{k} ");
        }
        Console.WriteLine("");
        int badkey = -1;
        int fury = -1;
        bool include = TestTest(rules, test, out badkey, out fury);

        if (include)
        {
            Console.WriteLine("ok");
        }
        else
        {
            // fix!
            var testest = new List<int>();
            foreach (var k in test) testest.Add(k);
            while (true)
            {
                var newtest = new List<int>();
                foreach (var k in testest)
                {
                    if (k != fury)
                    {
                        newtest.Add(k);
                    }
                    if (k == badkey)
                    {
                        newtest.Add(fury);
                    }
                }
                testest = newtest;
                include = TestTest(rules, testest, out badkey, out fury);
                if (include)
                    break;
                else
                {
                }
            }

            count++;
            int ind = testest.Count / 2;
            total += testest[ind];
        }
    }
}

static void Part1(Dictionary<int, List<int>> rules, List<List<int>> tests, out int count, out long total)
{
    count = 0;
    total = 0;
    foreach (var test in tests)
    {
        foreach (var k in test)
        {
            Console.Write($"{k} ");
        }
        Console.WriteLine("");
        int badkey = -1;
        int fury = -1;
        bool include = TestTest(rules, test, out badkey, out fury);

        if (include)
        {
            Console.WriteLine("ok");
            count++;
            int ind = test.Count / 2;
            total += test[ind];
        }
    }
}