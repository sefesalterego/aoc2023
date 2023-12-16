//Read the input
string[] input = File.ReadAllLines("Input.txt");
List<Galaxy> galaxies = [];
for (int y = 0; y < input.Length; y++)
{
	for (int x = 0; x < input[y].Length; x++)
	{
		if (input[y][x] == '#')
		{
			galaxies.Add(new(x, y));
		}
	}
}

//Expand the universe
List<Galaxy> Expand(List<Galaxy> galaxies, long factor)
{
	List<Galaxy> result = galaxies.Select(gal => gal with { }).ToList();
	long maxX = result.Max(gal => gal.X);
	long maxY = result.Max(gal => gal.Y);
	long expandBy = factor - 1;

	long deltaX = 0;
	for (long x = 0; x <= maxX; x++)
	{
		if (!result.Any(gal => gal.X == (x + deltaX)))
		{
			for (int index = 0; index < result.Count; index++)
			{
				if (result[index].X > (x + deltaX))
				{
					result[index].X += expandBy;
				}
			}
			deltaX += expandBy;
		}
	}

	long deltaY = 0;
	for (long y = 0; y <= maxY; y++)
	{
		if (!result.Any(gal => gal.Y == (y + deltaY)))
		{
			for (int index = 0; index < result.Count; index++)
			{
				if (result[index].Y > (y + deltaY))
				{
					result[index].Y += expandBy;
				}
			}
			deltaY += expandBy;
		}
	}

	return result;
}

List<Galaxy> expandedPt1 = Expand(galaxies, 2);
List<Galaxy> expandedPt2 = Expand(galaxies, 1_000_000);

//Determine the galaxy pairs
List<GalaxyPair> pairsPt1 = [];
List<GalaxyPair> pairsPt2 = [];
for (int index1 = 0; index1 < galaxies.Count; index1++)
{
	for (int index2 = index1 + 1; index2 < galaxies.Count; index2++)
	{
		pairsPt1.Add(new(expandedPt1[index1], expandedPt1[index2]));
		pairsPt2.Add(new(expandedPt2[index1], expandedPt2[index2]));
	}
}

//Calculate the distance
long CalcDistance(GalaxyPair pair) =>
	Math.Abs(pair.Galaxy1.X - pair.Galaxy2.X) + Math.Abs(pair.Galaxy1.Y - pair.Galaxy2.Y);

long totalDistancePt1 = 0;
foreach (GalaxyPair pair in pairsPt1)
{
	totalDistancePt1 += CalcDistance(pair);
}
Console.WriteLine("Part 1: " + totalDistancePt1);

long totalDistancePt2 = 0;
foreach (GalaxyPair pair in pairsPt2)
{
	totalDistancePt2 += CalcDistance(pair);
}
Console.WriteLine("Part 2: " + totalDistancePt2);

record Galaxy(long X, long Y) { public long X { get; set; } = X; public long Y { get; set; } = Y; };
record GalaxyPair(Galaxy Galaxy1, Galaxy Galaxy2);