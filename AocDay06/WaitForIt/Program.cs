using TimeDistance = (double time, double distance);
using WinningRange = (double min, double max);

string[] lines = File.ReadAllLines("Input.txt");

double[] Read(int lineIndex) =>
	lines[lineIndex].Split(' ', StringSplitOptions.RemoveEmptyEntries)[1..].Select(Double.Parse).ToArray();
double[] times = Read(0);
double[] distance = Read(1);

WinningRange CalcRecordRange(TimeDistance timeDist)
{
	double root = Math.Sqrt(timeDist.time * timeDist.time - 4 * timeDist.distance);
	return (Math.Floor((timeDist.time - root) / 2D) + 1D, Math.Ceiling((timeDist.time + root) / 2D) - 1D);
}

Console.WriteLine("Part 1: " +
	times.Zip(distance, (t, d) => (t, d))
		.Select(CalcRecordRange)
		.Aggregate(1D, (double agg, WinningRange range) => agg * (range.max - range.min + 1)));

double Parse(int lineIndex) =>
	Double.Parse(lines[lineIndex].Where(Char.IsDigit).Aggregate("", (string agg, char chr) => agg + chr));

WinningRange range = CalcRecordRange((Parse(0), Parse(1)));
Console.WriteLine("Part 2: " + (range.max - range.min + 1));
	