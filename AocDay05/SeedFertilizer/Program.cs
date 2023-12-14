string[] input = File.ReadAllLines("Input.txt");

IEnumerable<long> GetSeeds() =>
	input[0][(input[0].IndexOf(':') + 1)..]
		.Split(' ', StringSplitOptions.RemoveEmptyEntries)
		.Select(Int64.Parse);

MapSet[] mapSets = MapSet.Read(input);

long MapAllDown(long value)
{
	long current = value;
	foreach (MapSet mapSet in mapSets)
	{
		current = mapSet.MapDown(current);
	}
	return current;
}

//Part 1
Console.WriteLine("Part 1: " + GetSeeds().Min(MapAllDown));

//Part 2
long[] seeds = GetSeeds().ToArray();
(long min, long count)[] seedRanges = new (long,long)[seeds.Length / 2];
for (int rangeIndex = 0; rangeIndex < seedRanges.Length; rangeIndex++)
{
	int seedIndex = rangeIndex * 2;
	seedRanges[rangeIndex] = (seeds[seedIndex], seeds[seedIndex + 1]);
}

MapSet condensed = MapSet.Condense(mapSets);

var locationQry =
	from seedRange in seedRanges
	from locationRange in condensed.MapDown(seedRange.min, seedRange.count)
	select locationRange.min;
Console.WriteLine("Part 2: " + locationQry.Min());

public readonly struct Map(long downMin, long upMin, long count)
{
	private readonly long m_count = count;
	public long UpMin { get; } = upMin;
	public readonly long UpMax => UpMin + m_count - 1;
	public long DownMin { get; } = downMin;
	public readonly long DownMax => DownMin + m_count - 1;
	private readonly long UpToDownDelta => DownMin - UpMin;

	public long? MapDown(long upValue) =>
		(upValue >= UpMin) && (upValue <= UpMax) ? upValue + UpToDownDelta : null;

	public (Map merged, Map[] remaining)? MergeUp(Map up)
	{
		if ((up.DownMin > UpMax) || (up.DownMax < UpMin))
		{
			return null;
		}
		long minMiddle = Math.Max(up.DownMin, UpMin);
		long maxMiddle = Math.Min(up.DownMax, UpMax);
		long count = maxMiddle - minMiddle + 1;
		Map merged = new(minMiddle + UpToDownDelta, minMiddle - up.UpToDownDelta, count);

		List<Map> remaining = new();
		if (merged.DownMin > DownMin)
		{
			remaining.Add(new(DownMin, UpMin, merged.DownMin - DownMin));
		}
		if (merged.DownMax < DownMax)
		{
			remaining.Add(new(merged.DownMax + 1, merged.DownMax + 1 - UpToDownDelta, DownMax - merged.DownMax));
		}
		return (merged, remaining.ToArray());
	}

	public (long min, long max)? MapDown(long minUpValue, long maxUpValue) =>
		(minUpValue <= UpMax) && (maxUpValue >= UpMin) ?
			(Math.Max(minUpValue, UpMin) + UpToDownDelta, Math.Min(maxUpValue, UpMax) + UpToDownDelta) : null;

	public (long min, long max)[] GetRemaining(long minUpValue, long maxUpValue)
	{
		List<(long, long)> result = [];
		if (minUpValue < UpMin)
		{
			result.Add((minUpValue, Math.Min(UpMin - 1, maxUpValue)));
		}
		if (maxUpValue > UpMax)
		{
			result.Add((Math.Max(minUpValue, UpMax + 1), maxUpValue));
		}
		return result.ToArray();
	}
}

public readonly struct MapSet()
{
	private readonly List<Map> m_maps = new();

	private MapSet(IEnumerable<Map> maps) : this() => m_maps = new(maps);

	public void Add(Map map) => m_maps.Add(map);

	public long MapDown(long upValue)
	{
		foreach (Map map in m_maps)
		{
			long? downValue = map.MapDown(upValue);
			if (downValue is not null)
			{
				return downValue.Value;
			}
		}
		return upValue;
	}

	public (long min, long max)[] MapDown(long minUpValue, long count)
	{
		long maxUpValue = minUpValue + count - 1;
		List<(long, long)> result = [];
		List<(long, long)> upRanges = [(minUpValue, maxUpValue)];
		foreach (Map map in m_maps)
		{
			List<(long, long)> oldUpRanges = new(upRanges);
			upRanges.Clear();
			foreach ((long min, long max) in oldUpRanges)
			{
				(long, long)? range = map.MapDown(min, max);
				if (range is not null)
				{
					result.Add(range.Value);
					upRanges.AddRange(map.GetRemaining(min, max));
				}
				else
				{
					upRanges.Add((min, max));
				}
			}
			if (upRanges.Count == 0)
			{
				break;
			}
		}

		result.AddRange(upRanges);

		return result.ToArray();
	}

	private MapSet MergeUp(MapSet upper)
	{
		LinkedList<Map> unmapped = new(m_maps);
		List<Map> merged = [];

		Map? PickTop()
		{
			if (unmapped.Count == 0)
			{
				return null;
			}
			Map top = unmapped.First();
			unmapped.RemoveFirst();
			return top;
		}

		for (Map? current = PickTop(); current is not null; current = PickTop())
		{
			bool wasMerged = false;
			foreach (Map upperMap in upper.m_maps)
			{
				(Map merged, Map[] remaining)? result = current.Value.MergeUp(upperMap);
				if (result is not null)
				{
					wasMerged = true;
					merged.Add(result.Value.merged);
					foreach (Map remaining in result.Value.remaining)
					{
						unmapped.AddLast(remaining);
					}
					break;
				}
			}
			if (!wasMerged)
			{
				merged.Add(current.Value);
			}
		}

		return new(merged);
	}

	public static MapSet Condense(MapSet[] mapSets)
	{
		MapSet condensed = mapSets.Last();
		for (int index = mapSets.Length - 2; index >= 0; index--)
		{
			condensed = condensed.MergeUp(mapSets[index]);
		}
		return condensed;
	}

	public static MapSet[] Read(string[] input)
	{
		List<MapSet> mapSets = [];
		foreach (string line in input[1..])
		{
			if (String.IsNullOrWhiteSpace(line))
			{
				continue;
			}
			if (!Char.IsDigit(line[0]))
			{
				mapSets.Add(new());
				continue;
			}
			long[] values = line.Split().Select(Int64.Parse).ToArray();
			mapSets[mapSets.Count - 1].Add(new(values[0], values[1], values[2]));
		}
		return mapSets.ToArray();
	}
}
