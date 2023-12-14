using System.ComponentModel.DataAnnotations;

(int id, string content) GetGame(string line)
{
	string[] parts = line.Split(':');
	return (Int32.Parse(parts[0]["Game ".Length..]), parts[1]);
}

string[] GetCubesets(string content) => content.Split(';');

(int red, int green, int blue) GetCubes(string subset)
{
	string[] components = subset.Split(',');
	int red = 0, green = 0, blue = 0;
	foreach (string component in components.Select(comp => comp.Trim()))
	{
		int count = Int32.Parse(component[..component.IndexOf(' ')]);
		if (component.EndsWith("red"))
		{
			red += count;
		}
		else if (component.EndsWith("green"))
		{
			green += count;
		}
		else if (component.EndsWith("blue"))
		{
			blue += count;
		}
	}
	return (red, green, blue);
}

bool IsPossible((int red, int green, int blue) values) =>
	values.red <= 12 && values.green <= 13 && values.blue <= 14;

int CheckLine(string line)
{
	(int id, string content) = GetGame(line);
	return GetCubesets(content).All(cubeset => IsPossible(GetCubes(cubeset))) ? id : 0;
}

int GetPower(string line)
{
	(_, string content) = GetGame(line);
	(int red, int green, int blue) = GetCubesets(content)
		.Select(GetCubes)
		.Aggregate(((int red, int green, int blue) agg, (int red, int green, int blue) val) =>
			(Math.Max(val.red, agg.red), Math.Max(val.green, agg.green), Math.Max(val.blue, agg.blue)));
	return red * green * blue;
}

Console.WriteLine("Part 1: " +
	File.ReadAllLines("Input.txt")
		.Where(line => line.Length > 0)
		.Sum(CheckLine));

Console.WriteLine("Part 2: " +
	File.ReadAllLines("Input.txt")
		.Where(line => line.Length > 0)
		.Sum(GetPower));