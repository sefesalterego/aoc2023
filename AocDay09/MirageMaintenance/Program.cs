HistorySequence[] history = File.ReadAllLines("Input.txt").Select(line => new HistorySequence(line)).ToArray();

Console.WriteLine("Part 1: " + history.Select(hist => hist.GetNext()).Sum());
Console.WriteLine("Part 2: " + history.Select(hist => hist.GetPrevious()).Sum());

readonly struct HistorySequence(string line)
{
	private readonly List<int[]> m_values = BuildList(line);

	public readonly int GetPrevious()
	{
		int first = 0;
		foreach (int[] current in m_values.AsEnumerable().Reverse())
		{
			first = current.First() - first;
		}
		return first;
	}

	public readonly int GetNext()
	{
		int last = 0;
		foreach (int[] current in m_values.AsEnumerable().Reverse())
		{
			last += current.Last();
		}
		return last;
	}

	private static List<int[]> BuildList(string line)
	{
		List<int[]> values = [line.Split().Select(Int32.Parse).ToArray()];

		do
		{
			int[] lastLine = values.Last();
			int[] nextLine = new int[lastLine.Length - 1];

			for (int index = 1; index < lastLine.Length; index++)
			{
				nextLine[index - 1] = lastLine[index] - lastLine[index - 1];
			}
			values.Add(nextLine);
		} while (!values.Last().All(value => value == 0));
		return values;
	}
}