Value[] input = File.ReadAllLines("Input.txt")
	.SelectMany(line => line.Split(','))
	.Where(value => !String.IsNullOrWhiteSpace(value))
	.Select(value => new Value(value.Trim()))
	.ToArray();

Console.WriteLine("Part 1: " + input.Select(val => val.GetHashCode()).Sum());

//Put the lenses into the boxes
LinkedList<Lens>[] boxes = new LinkedList<Lens>[256];
for (int index = 0; index < boxes.Length; index++)
{
	boxes[index] = new LinkedList<Lens>();
}

foreach (Value value in input)
{
	Lens lens = new(value);
	LinkedList<Lens> lenses = boxes[lens.Box];
	bool hasSet = false;
	for (LinkedListNode<Lens>? node = lenses.First; node is not null; node = node.Next)
	{
		if (node.Value.Label == lens.Label)
		{
			if (lens.Focal is null)
			{
				lenses.Remove(node);
			}
			else
			{
				node.Value = lens;
			}
			hasSet = true;
			break;
		}
	}
	if (!hasSet && lens.Focal is not null)
	{
		lenses.AddLast(lens);
	}
}

//Calculate the total focal length
long focal = 0;
for (long boxIndex = 0; boxIndex < boxes.Length; boxIndex++)
{
	long slotIndex = 1;
	foreach(Lens lens in boxes[boxIndex])
	{
		focal += (boxIndex + 1) * slotIndex * lens.Focal!.Value;
		slotIndex++;
	}
}

Console.WriteLine("Part 2: " + focal);


struct Value(string raw)
{
	public readonly string Raw => raw;

	public override int GetHashCode()
	{
		int current = 0;
		foreach (char chr in raw)
		{
			unchecked
			{
				current += chr;
				current *= 17;
				current %= 256;
			}
		}
		return current;
	}
}

struct Lens
{
	private Value m_label;

	public string Label => m_label.Raw;
	public int Box => m_label.GetHashCode();
	public long? Focal { get; }

	public Lens(Value input)
	{
		int labelLength = input.Raw.IndexOfAny(['=', '-']);
		m_label = new Value(input.Raw[..labelLength]);
		if (input.Raw[labelLength] == '=')
		{
			Focal = input.Raw[labelLength + 1] - '0';
		}
        else
        {
			Focal = null;
        }
    }
}