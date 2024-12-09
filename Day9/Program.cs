// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
List<Block> blocks = new();
List<int> spaces = new();
var lines = File.ReadAllLines(args[0]);
int count = 0;
int indexc = 0;
for (int i = 0; i < lines[0].Length; i += 2)
{
    int blocksize = int.Parse($"{lines[0][i]}");
    blocks.Add(new Block(blocksize, count, indexc));
    count++;
    indexc += blocksize;
    if (i + 1 < lines[0].Length)
    {
        int spacesize = int.Parse($"{lines[0][i + 1]}");
        spaces.Add(spacesize);
        indexc += spacesize;
    }
}

Console.WriteLine($"{blocks.Count} blocks interspersed with {spaces.Count} spaces.");

Part2(blocks, spaces);
// Part1(blocks, spaces);

/**
 * Ok, similar idea, but now gives each block an index, and we keep position of where
 * we are on the disk (positionCounter). Start at one end, taking blocks and spaces in
 * turn, but now search from the end to the beginnng for blocks to fit the space. This
 * means as we move blocks we re-index them.
 * 
 * At the end, reorder the blocks by their index, and then use the Id and Index to
 * calculate the total.
 */
static void Part2(List<Block> blocks, List<int> spaces)
{
    int positionCounter = 0;
    int spaceCounter = 0;
    int startBlock = 0;
    long total = 0;

    while (spaceCounter < spaces.Count)
    {
        if (startBlock >= blocks.Count)
            break;

        Block ffull = blocks[startBlock];

        // ffull's index doesn't change.
        positionCounter += ffull.C; // C is the size of the block.

        int space = spaces[spaceCounter];
        // spaces is the size of the end block. So fit while we can?
        int endBlock = blocks.Count - 1;
        while (startBlock < endBlock && space > 0)
        {
            if (blocks[endBlock].Index > positionCounter && blocks[endBlock].C <= space)
            {
                blocks[endBlock].Index = positionCounter;
                space -= blocks[endBlock].C;
                positionCounter += blocks[endBlock].C;
            }
            endBlock--;
        }

        // We jump to the next block I think.
        positionCounter += space; // there may be space remaining that isn't catered for.
        startBlock += 1;
        spaceCounter++;
    }

    blocks.Sort(new blockCompare());

    foreach (Block block in blocks)
    {
        Console.WriteLine($"{block.Id} {block.C} {block.Index}");
        for (int k = block.Index; k < block.Index + block.C; k++)
        {
            total += (long)(block.Id * k);
        }
    }
    Console.WriteLine(total);
    Console.WriteLine("what now?");
}

/**
 * Starts at the beginning - taking a block, then shuffling from the end
 * blocks in the next space. Keep going until the start/end block overlap.
 */
static void Part1(List<Block> blocks, List<int> spaces)
{
    int positionCounter = 0;
    int spaceCounter = 0;
    int startBlock = 0;
    int endBlock = blocks.Count - 1;
    long total = 0;

    while (true)
    {
        Block ffull = blocks[startBlock];
        int space = spaces[spaceCounter];

        for (int k = 0; k < ffull.C; k++)
        {
            total += (long)(ffull.Id * positionCounter);
            positionCounter++;
            Console.Write(ffull.Id);
        }
        ffull.C = 0;

        while (endBlock > startBlock)
        {
            Block efull = blocks[endBlock];
            int c = efull.C;
            int s = space;
            space = efull.Decrement(space);
            for (int l = 0; l < s - space; l++)
            {
                total += (long)(positionCounter * efull.Id);
                positionCounter++;
                Console.Write(efull.Id);
            }
            if (efull.C == 0)
                endBlock--;
            if (space == 0)
                break;
        }
        startBlock++;
        spaceCounter++;
        if (spaceCounter >= spaces.Count)
            break;
    }

    // finish off blocks.
    while (startBlock <= endBlock)
    {
        var rem = blocks[endBlock];
        for (int k = 0; k < rem.C; k++)
        {
            total += (long)(rem.Id * positionCounter);
            positionCounter++;
            Console.Write(rem.Id);
        }
        endBlock--;
    }

    Console.WriteLine("");
    Console.WriteLine($"{total}");
}

/**
 * Compare blocks by their index.
 */
public class blockCompare : IComparer<Block>
{
    public int Compare(Block? x, Block? y)
    {
        if (x == null || y == null) throw new ArgumentNullException("x");
        return x.Index - y.Index;
    }
}

public class Block
{
    public int C { get; set; }
    public int Id { get; set; }
    public int Index { get; set; }

    public Block(int c, int id, int index)
    {
        C = c;
        Id = id;
        Index = index; 
    }

    // Decrements the count but if there was any left over then return it so
    // we can decrement from another block.
    public int Decrement(int x)
    {
        if (x > C)
        {
            int g = x - C;
            C = 0;
            return g;
        }
        C = C - x;
        return 0;

    }
}