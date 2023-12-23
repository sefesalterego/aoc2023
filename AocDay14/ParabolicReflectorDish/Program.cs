using System.Diagnostics.CodeAnalysis;

//Parse the input
string[] lines = File.ReadAllLines("Input.txt")
	.Where(line => !String.IsNullOrWhiteSpace(line))
	.ToArray();
Pattern pattern = new(lines);
pattern.MoveNorth();


//Calculate the weight (part 1)
Console.WriteLine("Part 1: " + pattern.WeightOnNorthBeam);

//Part 2
List<Pattern> patterns = [pattern];

//Cycle and compare to previous cycles
int cycleIndex = -1;

while (cycleIndex < 0)
{
	Pattern next = patterns[patterns.Count - 1].Cycle();
	for (int index = 0; index < patterns.Count; index++)
	{
		if (next.Equals(patterns[index]))
		{
			cycleIndex = index;
			break;
		}
	}
	if (cycleIndex < 0)
	{
		patterns.Add(next);
	}
}

//Get the index after 1000000000 cycles
int patternIndex = cycleIndex + ((1_000_000_000 - cycleIndex) % (patterns.Count - cycleIndex));
Console.WriteLine("Part 2: " + patterns[patternIndex].WeightOnNorthBeam);

struct Pattern
{
	private readonly char[,] m_value;

	public Pattern(string[] lines)
	{
		m_value = new char[lines[0].Length, lines.Length];
		for (int y = 0; y < lines.Length; y++)
		{
			for (int x = 0; x < lines[y].Length; x++)
			{
				m_value[x, y] = lines[y][x];
			}
		}
	}

	private Pattern(char[,] value) => m_value = value;

	public int WeightOnNorthBeam
	{
		get
		{
			int totalWeight = 0;
			for (int y = 0; y < m_value.GetLength(1); y++)
			{
				for (int x = 0; x < m_value.GetLength(0); x++)
				{
					if (m_value[x, y] == 'O')
					{
						totalWeight += m_value.GetLength(1) - y;
					}
				}
			}
			return totalWeight;
		}
	}

	public readonly Pattern Cycle()
	{
		char[,] cloneValue = new char[m_value.GetLength(0), m_value.GetLength(1)];
		Array.Copy(m_value, cloneValue, m_value.Length);
		Pattern clone = new(cloneValue);

		clone.MoveNorth();
		clone.MoveWest();
		clone.MoveSouth();
		clone.MoveEast();

		return clone;
	}

	public void MoveNorth()
	{
		for (int x = 0; x < m_value.GetLength(0); x++)
		{
			int freeAbove = 0;
			for (int y = 0; y < m_value.GetLength(1); y++)
			{
				switch (m_value[x, y])
				{
					case '.':
						freeAbove++;
						break;

					case 'O':
						if (freeAbove > 0)
						{
							m_value[x, y] = '.';
							m_value[x, y - freeAbove] = 'O';
						}
						break;

					default:
						freeAbove = 0;
						break;

				}
			}
		}
	}

	void MoveWest()
	{
		for (int y = 0; y < m_value.GetLength(1); y++)
		{
			int freeBefore = 0;
			for (int x = 0; x < m_value.GetLength(0); x++)
			{
				switch (m_value[x, y])
				{
					case '.':
						freeBefore++;
						break;

					case 'O':
						if (freeBefore > 0)
						{
							m_value[x, y] = '.';
							m_value[x - freeBefore, y] = 'O';
						}
						break;

					default:
						freeBefore = 0;
						break;

				}
			}
		}
	}

	void MoveSouth()
	{
		for (int x = 0; x < m_value.GetLength(0); x++)
		{
			int freeBelow = 0;
			for (int y = m_value.GetLength(1) - 1; y >= 0; y--)
			{
				switch (m_value[x, y])
				{
					case '.':
						freeBelow++;
						break;

					case 'O':
						if (freeBelow > 0)
						{
							m_value[x, y] = '.';
							m_value[x, y + freeBelow] = 'O';
						}
						break;

					default:
						freeBelow = 0;
						break;

				}
			}
		}
	}

	void MoveEast()
	{
		for (int y = 0; y < m_value.GetLength(1); y++)
		{
			int freeAfter = 0;
			for (int x = m_value.GetLength(0) - 1; x >= 0; x--)
			{
				switch (m_value[x, y])
				{
					case '.':
						freeAfter++;
						break;

					case 'O':
						if (freeAfter > 0)
						{
							m_value[x, y] = '.';
							m_value[x + freeAfter, y] = 'O';
						}
						break;

					default:
						freeAfter = 0;
						break;

				}
			}
		}
	}

	public override readonly bool Equals([NotNullWhen(true)] object? obj) =>
		obj is Pattern other && m_value.Cast<char>().SequenceEqual(other.m_value.Cast<char>());

	public override readonly int GetHashCode() =>
		m_value.Cast<char>().Aggregate(0, (int agg, char val) => HashCode.Combine(agg, val.GetHashCode()));
}