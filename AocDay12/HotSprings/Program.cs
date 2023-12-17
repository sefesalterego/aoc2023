//Read the data
Row[] rows = File.ReadAllLines("Input.txt")
	.Where(line => !String.IsNullOrWhiteSpace(line))
	.Select(line => new Row(line))
	.ToArray();

int test = rows[1].CountCombinations();
Console.WriteLine("Part 1: " + rows.Sum(row => row.CountCombinations()));

enum CheckResult { Inconclusive, Success, Fail }

struct Row
{
	private const char OK = '.';
	private const char BROKEN = '#';
	private const char UNKNOWN = '?';

	public char[] Pattern { get; }
	public int[] GroupSizes { get; }

	public Row(string input)
	{
		string[] content = input.Split();
		Pattern = content[0].ToCharArray();
		GroupSizes = content[1].Split(',').Select(Int32.Parse).ToArray();
	}

	private Row(char[] pattern, int[] groupSizes)
	{
		Pattern = pattern;
		GroupSizes = groupSizes;
	}

	private bool TrySetNext(char state, int count = 1)
	{
		if (count < 1)
		{
			return true;
		}

		for (int index = 0; index < Pattern.Length; index++)
		{
			if (Pattern[index] == UNKNOWN)
			{
				Pattern[index] = state;
				count--;
				if (count == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private (int size, bool isFinal) GetActualGroupSize(int groupIndex)
	{
		int currentGroupIndex = -1;
		int currentGroupSize = 0;
		for (int index = 0; index < Pattern.Length; index++)
		{
			if (Pattern[index] == BROKEN)
			{
				if (currentGroupSize == 0)
				{
					currentGroupIndex++;
					currentGroupSize = 1;
				}
				else
				{
					currentGroupSize++;
				}
			}
			else if (currentGroupIndex == groupIndex)
			{
				return (currentGroupSize, Pattern[index] == OK);
			}
			else if (Pattern[index] == UNKNOWN)
			{
				return (0, false);
			}
			else
			{
				currentGroupSize = 0;
			}
		}

		if (currentGroupIndex == groupIndex)
		{
			return (currentGroupSize, true);
		}
		else
		{
			return (0, true);
		}
	}

	private bool IsCompleted()
	{
		if (Pattern.Contains(UNKNOWN))
		{
			return false;
		}
		bool inGroup = false;
		int groupCount = 0;
		for (int index = 0; index < Pattern.Length; index++)
		{
			if (Pattern[index] == OK)
			{
				inGroup = false;
			}
			else
			{
				if (!inGroup)
				{
					groupCount++;
				}
				inGroup = true;
			}
		}
		return groupCount == GroupSizes.Length;
	}
	public int CountCombinations(int groupIndex = 0, List<string>? successRows = null)
	{
		//Determine the result with brute force
		int successes = 0;
		successRows ??= [];

		for (int okCount = 0; okCount < Pattern.Length; okCount++)
		{
			Row current = Clone();
			if (!current.TrySetNext(OK, okCount))
			{
				return successes;
			}

			int targetSize = current.GroupSizes[groupIndex];

			CheckResult result = CheckResult.Inconclusive;
			while (result == CheckResult.Inconclusive)
			{
				(int size, bool isFinal) = current.GetActualGroupSize(groupIndex);
				if (size == targetSize)
				{
					if (!isFinal)
					{
						_ = current.TrySetNext(OK);
					}
					result = CheckResult.Success;
				}
				else if (isFinal || (size > targetSize))
				{
					result = CheckResult.Fail;
				}
				else
				{
					_ = current.TrySetNext(BROKEN);
				}
			}
			if (result == CheckResult.Fail)
			{
				continue;
			}

			if (groupIndex == (GroupSizes.Length - 1))
			{
				while (current.TrySetNext(OK)) { }
				if (current.IsCompleted())
				{
					string asText = current.ToString();
					if (successRows.Contains(asText))
					{
						continue;
					}
					successes++;
					successRows.Add(asText);
				}
				continue;
			}
			successes += current.CountCombinations(groupIndex + 1, successRows);
		}
		return successes;
	}

	private Row Clone()
	{
		char[] pattern = new char[Pattern.Length];
		int[] groupSizes = new int[GroupSizes.Length];
		Array.Copy(Pattern, pattern, pattern.Length);
		Array.Copy(GroupSizes, groupSizes, groupSizes.Length);
		return new Row(pattern, groupSizes);
	}

	public override readonly string ToString() => new(Pattern);
}