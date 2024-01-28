//Read the input
string[] lines = File.ReadAllLines("Input.txt");
Tile[,] tiles = new Tile[lines[0].Length, lines.Length];
for (int x = 0; x < tiles.GetLength(0); x++)
{
	for (int y = 0; y < tiles.GetLength(1); y++)
	{
		tiles[x, y] = new Tile(lines[y][x]);
	}
}

int GetEnergizedCount(Beam initial, bool withOutput = false)
{
	Tile[,] energizeTiles = new Tile[tiles.GetLength(0), tiles.GetLength(1)];
	for (int x = 0; x < tiles.GetLength(0); x++)
	{
		for (int y = 0; y < tiles.GetLength(1); y++)
		{
			energizeTiles[x, y] = tiles[x, y].Clone();
		}
	}

	//Fire the beam
	Queue<Beam> beams = [];
	beams.Enqueue(initial);

	do
	{
		Beam? beam = beams.Dequeue();
		while (beam is not null)
		{
			if (beam.Value.X < 0 || beam.Value.X >= energizeTiles.GetLength(0) ||
				beam.Value.Y < 0 || beam.Value.Y >= energizeTiles.GetLength(1))
			{
				//Beam has left the grid
				beam = null;
				continue;
			}

			Beam[] exitingBeams = energizeTiles[beam.Value.X, beam.Value.Y].Energize(beam.Value);
			if (exitingBeams.Length == 0)
			{
				//Beam has already traveled through the tile from the direction
				beam = null;
				continue;
			}
			if (exitingBeams.Length > 1)
			{
				//If the beam was split, continue with one of the exiting beams and remember the other for later
				beams.Enqueue(exitingBeams[1]);
			}
			beam = exitingBeams[0];
		}
	} while (beams.Count > 0);

	//Count the energized tiles
	int energized = 0;
	for (int y = 0; y < energizeTiles.GetLength(1); y++)
	{
		for (int x = 0; x < energizeTiles.GetLength(0); x++)
		{
			if (energizeTiles[x, y].IsEnergized)
			{
				if (withOutput)
				{
					Console.Write("#");
				}
				energized++;
			}
			else if (withOutput)
			{
				Console.Write(".");
			}
		}
		if (withOutput)
		{
			Console.WriteLine();
		}
	}
	if (withOutput)
	{
		Console.WriteLine();
	}
	return energized;
}

//Part 1:
Console.WriteLine("Part 1: " + GetEnergizedCount(new Beam(Direction.Left, 0, 0)), true);

//Part 2:
int maxEnergized = 0;
for (int x = 0; x < tiles.GetLength(0); x++)
{
	int top = GetEnergizedCount(new Beam(Direction.Top, x, 0));
	int bottom = GetEnergizedCount(new Beam(Direction.Bottom, x, tiles.GetLength(1) - 1));
	maxEnergized = Math.Max(maxEnergized, Math.Max(top, bottom));
}

for (int y = 0; y < tiles.GetLength(1); y++)
{
	int left = GetEnergizedCount(new Beam(Direction.Left, 0, y));
	int right = GetEnergizedCount(new Beam(Direction.Right, tiles.GetLength(0) - 1, y));
	maxEnergized = Math.Max(maxEnergized, Math.Max(left, right));
}
Console.WriteLine("Part 2: " + maxEnergized);


[Flags] enum Direction { Top = 0b0001, Bottom = 0b0010, Left = 0b0100, Right = 0b1000 }

record struct Beam(Direction From, int X, int Y);

class Tile(char device)
{
	private Direction m_energizedFrom;
	public char Device { get; } = device;
	public bool IsEnergized => m_energizedFrom != 0;

	public Beam[] Energize(Beam beam)
	{
		//Do not process if result is already known
		if ((beam.From & m_energizedFrom) != 0)
		{
			return [];
		}

		//Get the from direction of the exiting beams
		switch (Device)
		{
			case '/':
				m_energizedFrom |= beam.From;
				return beam.From switch
				{
					Direction.Top => [new Beam(Direction.Right, beam.X - 1, beam.Y)],
					Direction.Bottom => [new Beam(Direction.Left, beam.X + 1, beam.Y)],
					Direction.Left => [new Beam(Direction.Bottom, beam.X, beam.Y - 1)],
					_ => [new Beam(Direction.Top, beam.X, beam.Y + 1)],
				};
			case '\\':
				m_energizedFrom |= beam.From;
				return beam.From switch
				{
					Direction.Top => [new Beam(Direction.Left, beam.X + 1, beam.Y)],
					Direction.Bottom => [new Beam(Direction.Right, beam.X - 1, beam.Y)],
					Direction.Left => [new Beam(Direction.Top, beam.X, beam.Y + 1)],
					_ => [new Beam(Direction.Bottom, beam.X, beam.Y - 1)],
				};
			case '|':
				switch (beam.From)
				{
					case Direction.Top:
						m_energizedFrom |= beam.From;
						return [new Beam(Direction.Top, beam.X, beam.Y + 1)];

					case Direction.Bottom:
						m_energizedFrom |= beam.From;
						return [new Beam(Direction.Bottom, beam.X, beam.Y - 1)];

					default:
						m_energizedFrom |= Direction.Left | Direction.Right;
						return [new Beam(Direction.Top, beam.X, beam.Y + 1),
							new Beam(Direction.Bottom, beam.X, beam.Y - 1)];
				}

			case '-':
				switch (beam.From)
				{
					case Direction.Left:
						m_energizedFrom |= beam.From;
						return [new Beam(Direction.Left, beam.X + 1, beam.Y)];

					case Direction.Right:
						m_energizedFrom |= beam.From;
						return [new Beam(Direction.Right, beam.X - 1, beam.Y)];

					default:
						m_energizedFrom |= Direction.Top | Direction.Bottom;
						return [new Beam(Direction.Left, beam.X + 1, beam.Y),
							new Beam(Direction.Right, beam.X - 1, beam.Y)];
				}

			default:
				m_energizedFrom |= beam.From;
				return beam.From switch
				{
					Direction.Top => [new Beam(Direction.Top, beam.X, beam.Y + 1)],
					Direction.Bottom => [new Beam(Direction.Bottom, beam.X, beam.Y - 1)],
					Direction.Left => [new Beam(Direction.Left, beam.X + 1, beam.Y)],
					_ => [new Beam(Direction.Right, beam.X - 1, beam.Y)],
				};
		}
	}

	public Tile Clone()
	{
		return new Tile(Device);
	}
}