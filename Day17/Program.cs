// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Threading.Tasks.Sources;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
var A = long.Parse(lines[0].Split(':', StringSplitOptions.RemoveEmptyEntries)[1]);
var B = long.Parse(lines[1].Split(':', StringSplitOptions.RemoveEmptyEntries)[1]);
var C = long.Parse(lines[2].Split(':', StringSplitOptions.RemoveEmptyEntries)[1]);

var program = lines[4].Split(':', StringSplitOptions.RemoveEmptyEntries)[1];
var vals = program.Split(',');
int[] ops = new int[vals.Length];
for (int i = 0; i < ops.Length; i++) ops[i] = int.Parse(vals[i]);

// This bit does part 2.
var newA = Part2Iterative(A, B, C, vals, ops, ops);
Console.WriteLine($"new A is {newA}");

// This bit uses the output from above to make sure that we get the same
// output as the program itself.
// It can also be used just to run Part 1, if you comment out the above
// part 2 bit, and the A = newA; line.
Console.WriteLine(program);
A = newA;
var nums = Part1(ref A, ref B, ref C, vals, ops);
Console.Write($"{nums[0]}");
for (int p = 1; p < nums.Length; p++) Console.Write($",{nums[p]}");
Console.WriteLine();

long Part2Iterative(long A, long B, long C, string[] vals, int[] ops, int[] target)
{
    return calculateLowest(0, target.Length - 1, B, C, vals, ops, target);
}

long calculateLowest(long value, int index, long B, long C, string[] vals, int[] ops, int[] target)
{
    if (index < 0)
    {
        return value;
    }

    long minValue = value * 8;
    long maxValue = value * 8 + 7;

    for (long testValue = minValue; testValue <= maxValue; testValue++)
    {
        var testvaluecopy = testValue;
        var bcopy = B;
        var ccopy = C;

        var arr = Part1(ref testvaluecopy, ref bcopy, ref ccopy, vals, ops);
        if (arr[0] == target[index])
        {
            Console.WriteLine($"step {index} of {testValue}");
            long result = calculateLowest(testValue, index - 1, B, C, vals, ops, target);

            if (result > 0)
            {
                return result;
            }
        }
    }

    return 0;
}

long calculateComboOperand(int op, long A, long B, long C)
{
    if (op <= 3) return op;
    if (op == 4) return A;
    if (op == 5) return B;
    if (op == 6) return C;
    return -1;
}

int[] Part1(ref long A, ref long B, ref long C, string[] vals, int[] ops)
{
    int instPointer = 0;
    List<int> output = new();

    while (true)
    {
        Console.WriteLine($"A={A} B={B} C={C}");
        if (instPointer >= vals.Length)
            break;

        long comboOp = calculateComboOperand(ops[instPointer + 1], A, B, C);
        long literalOp = ops[instPointer + 1];
        switch (ops[instPointer])
        {
            case 0: // adv
                A = (long)(A / Math.Pow(2, comboOp));
                instPointer += 2;
                break;

            case 1: // bxl
                B = B ^ literalOp;
                instPointer += 2;
                break;

            case 2: // bst
                var newB = comboOp % 8;
                B = newB;
                instPointer += 2;
                break;

            case 3: // jnz
                if (A == 0)
                {
                    instPointer += 2;
                }
                else
                {
                    instPointer = (int)literalOp;
                }
                break;

            case 4: // bxc
                B = B ^ C;
                instPointer += 2;
                break;

            case 5: // out
                //Console.Write($"{comboOp % 8},");
                output.Add((int)(comboOp % 8));
                instPointer += 2;
                break;

            case 6:
                B = (long)(A / Math.Pow(2, comboOp));
                instPointer += 2;
                break;

            case 7:
                C = (long)(A / Math.Pow(2, comboOp));
                instPointer += 2;
                break;

            default:
                throw new Exception("floundering.");
        }
    }

    //Console.WriteLine("");
    //Console.WriteLine($"A = {A}  B = {B}  C = {C}");
    return output.ToArray();
}