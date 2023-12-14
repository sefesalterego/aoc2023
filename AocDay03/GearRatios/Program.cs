using System.Text;
using Part = (int partNum, int xMin, int xMax, int y);

(bool[,] symbolMap, bool[,] gearMap, Part[] parts) ParseMap(string[] lines)
{
	bool[,] symbolMap = new bool[lines[0].Length, lines.Length];
	bool[,] gearMap = new bool[lines[0].Length, lines.Length];
	List<(int partNum, int xMin, int xMax, int y)> parts = [];

	for (int y = 0; y < symbolMap.GetLength(1); y++)
	{
		StringBuilder numBuilder = new();
		int? xMin = null;
		for (int x = 0; x < lines[y].Length; x++)
		{
			char chr = lines[y][x];

			symbolMap[x, y] = chr != '.' && !Char.IsDigit(chr);
			gearMap[x, y] = chr == '*';

			if (Char.IsDigit(chr))
			{
				numBuilder.Append(chr);
				if (xMin is null)
				{
					xMin = x;
				}
			}
			else if (numBuilder.Length > 0)
			{
				int partNum = Int32.Parse(numBuilder.ToString());
				parts.Add((partNum, xMin ?? throw new InvalidOperationException(), x - 1, y));
				numBuilder.Clear();
				xMin = null;
			}
		}

		if (numBuilder.Length > 0)
		{
			int partNum = Int32.Parse(numBuilder.ToString());
			parts.Add((partNum, xMin ?? throw new InvalidOperationException(), lines[y].Length - 1, y));
		}
	}

	return (symbolMap, gearMap, parts.ToArray());
}

(int xMin, int xMax, int yMin, int yMax) GetBox(bool[,] symbolMap, Part part)
{
	int xMin = Math.Max(0, part.xMin - 1);
	int xMax = Math.Min(symbolMap.GetLength(0) - 1, part.xMax + 1);
	int yMin = Math.Max(0, part.y - 1);
	int yMax = Math.Min(symbolMap.GetLength(1) - 1, part.y + 1);
	return (xMin, xMax, yMin, yMax);
}

bool IsPartNumber(bool[,] symbolMap, Part part)
{
	(int xMin, int xMax, int yMin, int yMax) = GetBox(symbolMap, part);
	for (int x = xMin; x <= xMax; x++)
	{
		for (int y = yMin; y <= yMax; y++)
		{
			if (symbolMap[x, y])
			{
				return true;
			}
		}
	}
	return false;
}

(int partNum, int x, int y)[] GetGears(bool[,] gearMap, Part part)
{
	(int xMin, int xMax, int yMin, int yMax) = GetBox(gearMap, part);
	List<(int partNum, int x, int y)> result = new();
	for (int x = xMin; x <= xMax; x++)
	{
		for (int y = yMin; y <= yMax; y++)
		{
			if (gearMap[x, y])
			{
				result.Add((part.partNum, x, y));
			}
		}
	}
	return result.ToArray();
}

(var symbolMap, var gearMap, var parts) = ParseMap(File.ReadAllLines("Input.txt"));

Console.WriteLine("Part 1: " + parts.Where(part => IsPartNumber(symbolMap, part)).Sum(part => part.partNum));

var gearRatioQry =
	from part in parts
	let gears = GetGears(gearMap, part)
	from gear in gears
	group gear.partNum by (gear.x, gear.y) into gearGrp
	where gearGrp.Count() == 2
	select gearGrp.ElementAt(0) * gearGrp.ElementAt(1);
Console.WriteLine("Part 2: " + gearRatioQry.Sum());