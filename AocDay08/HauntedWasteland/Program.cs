using System.Collections;

string[] input = File.ReadAllLines("Input.txt");

Map map = new(input[0]);

Dictionary<string, Node> nodes = [];
foreach (string line in input[1..].Where(line => !String.IsNullOrWhiteSpace(line)))
{
	Node node = new(line, nodes, map);
	nodes.Add(node.Code, node);
}

int steps = 0;
Node current = nodes["AAA"];
foreach (char direction in map)
{
	steps++;
	current = current.GoForward(direction);
	if (current.Code == "ZZZ")
	{
		break;
	}
}

Console.WriteLine("Part 1: " + steps);

long GreatestCommonFactor (long a, long b)
{
	while (b != 0)
	{
		long mod = a % b;
		a = b;
		b = mod;
	}
	return a;
}

long SmallestCommonMultiple(long a, long b) => a / GreatestCommonFactor(a, b) * b;

long[] stepCounts =
	nodes.Where(entry => entry.Key.EndsWith('A')).Select(node => node.Value.GetNextZ(0).totalSteps).ToArray();

long multiple = stepCounts[0];
for (int index = 1; index < stepCounts.Length; index++)
{
	multiple = SmallestCommonMultiple(multiple, stepCounts[index]);
}

Console.WriteLine("Part 2: " + multiple);

readonly struct Map(string line) : IEnumerable<char>
{
	public string Line { get; } = line;
	public int Length => Line.Length;

	public char this[long index] => Line[(int)(index % Line.Length)];

	public IEnumerator<char> GetEnumerator()
	{
		int index = 0;
		while (true)
		{
			if (index >= Line.Length)
			{
				index = 0;
			}
			yield return Line[index];
			index++;
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

class Node
{
	private readonly IDictionary<string, Node> m_nodes;
	private readonly Map m_map;
	private readonly string m_leftCode;
	private readonly string m_rightCode;
	private Node? m_left;
	private Node? m_right;

	public string Code { get; }

	public Node Left
	{
		get
		{
			if (m_left == null)
			{
				m_left = m_nodes[m_leftCode];
			}
			return m_left;
		}
	}

	public Node Right
	{
		get
		{
			if (m_right == null)
			{
				m_right = m_nodes[m_rightCode];
			}
			return m_right;
		}
	}

	public Node(string line, IDictionary<string, Node> nodes, Map map)
	{
		m_nodes = nodes;
		m_map = map;
		string[] parts = line.Split('=');

		Code = parts[0].Trim();

		string[] leftRight =
			parts[1].Trim().TrimStart('(').TrimEnd(')').Split(',').Select(subPart => subPart.Trim()).ToArray();
		m_leftCode = leftRight[0];
		m_rightCode = leftRight[1];
	}

	public Node GoForward(char direction) => direction == 'L' ? Left : Right;

	public (Node node, long totalSteps) GetNextZ(long currentStep)
	{
		long mapIndex = currentStep;
		Node current = this;
		do {
			current = current.GoForward(m_map[mapIndex]);
			mapIndex++;
		}
		while (!current.Code.EndsWith('Z'));

		return (current, mapIndex);
	}
}
