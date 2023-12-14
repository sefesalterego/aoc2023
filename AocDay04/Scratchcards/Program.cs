HashSet<int> ReadNumbers(string numbers) =>
	new(numbers.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(num => Int32.Parse(num.Trim())));

int GetWinningCount(string line)
{
	string winning = line[(line.IndexOf(':') + 1)..line.IndexOf('|')];
	string actual = line[(line.IndexOf('|') + 1)..];
	int winningCount = ReadNumbers(winning).Intersect(ReadNumbers(actual)).Count();
	return winningCount;
}

int GetScore(string line)
{
	int winningCount = GetWinningCount(line);
	return (winningCount != 0) ? 1 << (winningCount - 1) : 0;
}

var linesQry = File.ReadAllLines("Input.txt")
	.Where(line => line.Length > 0);
string[] lines = linesQry.ToArray();

Console.WriteLine("Part 1: " + lines.Sum(GetScore));

int[] cardCount = new int[lines.Length];

for (int line = 0; line < lines.Length; line++)
{
	cardCount[line] = cardCount[line] + 1;
	int winningCount = GetWinningCount(lines[line]);
	for (int following = line + 1; following < Math.Min(cardCount.Length, line + winningCount + 1); following++)
	{
		cardCount[following] = cardCount[following] + cardCount[line];
	}
}

Console.WriteLine("Part 2: " + cardCount.Sum());