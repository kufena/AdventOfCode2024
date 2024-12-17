// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
var A = long.Parse(lines[0].Split(':', StringSplitOptions.RemoveEmptyEntries)[1]);
var B = long.Parse(lines[1].Split(':', StringSplitOptions.RemoveEmptyEntries)[1]);
var C = long.Parse(lines[2].Split(':', StringSplitOptions.RemoveEmptyEntries)[1]);

var vals = lines[4].Split(':')[1].Split(',');
int[] ops = new int[vals.Length];
for(int i = 0; i < ops.Length; i++) ops[i]= int.Parse(vals[i]);

int instPointer = 0;

while(true)
{
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
            Console.Write($"{comboOp % 8},");
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

Console.WriteLine("");
Console.WriteLine($"A = {A}  B = {B}  C = {C}");

long calculateComboOperand(int op, long A, long B, long C)
{
    if (op <= 3) return op;
    if (op == 4) return A;
    if (op == 5) return B;
    if (op == 6) return C;
    return -1;
}
