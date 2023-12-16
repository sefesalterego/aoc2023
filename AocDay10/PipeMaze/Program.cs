//Create a dictionary to read the symbols from the input and convert them to pipes
Dictionary<char, Pipes> symbolToPipes = new() { {'|', Pipes.North | Pipes.South }, {'-', Pipes.East | Pipes.West },
	{'L', Pipes.North | Pipes.East }, {'J', Pipes.North | Pipes.West }, {'7', Pipes.South | Pipes.West },
	{'F', Pipes.East | Pipes.South }, {'.', Pipes.None }, {'S', Pipes.North | Pipes.East | Pipes.South | Pipes.West } };

//Read the input
int startX = 0, startY = 0;
string[] lines = File.ReadAllLines("Input.txt");
Pipes[,] pipeMap = new Pipes[lines[0].Length, lines.Length];
for (int y = 0; y < lines.Length; y++)
{
	for (int x = 0; x < lines[y].Length; x++)
	{
		pipeMap[x, y] = symbolToPipes[lines[y][x]];
		if (lines[y][x] == 'S')
		{
			startX = x;
			startY = y;
		}
	}
}

//Get the starting directions
bool CanGo(int currentX, int currentY, Pipes to) =>
	to switch
	{
		Pipes.North => (currentY > 0) && (pipeMap[currentX, currentY - 1] & Pipes.South) > 0,
		Pipes.East => (currentX < pipeMap.GetLength(0) - 1) && (pipeMap[currentX + 1, currentY] & Pipes.West) > 0,
		Pipes.South => (currentY < pipeMap.GetLength(1) - 1) && (pipeMap[currentX, currentY + 1] & Pipes.North) > 0,
		_ => (currentX > 0) && (pipeMap[currentX - 1, currentY] & Pipes.East) > 0,
	};

Pipes[] directions = [Pipes.North, Pipes.East, Pipes.South, Pipes.West];
Pipes direction1 = directions.First(dir => CanGo(startX, startY, dir));
Pipes direction2 = directions.Last(dir => CanGo(startX, startY, dir));
pipeMap[startX, startY] = direction1 | direction2;

//Move through pipes
Pipes GetFrom(Pipes to) =>
	to switch { Pipes.North => Pipes.South, Pipes.East => Pipes.West, Pipes.South => Pipes.North, _ => Pipes.East };

(int x, int y, Pipes to) GoForward(int currentX, int currentY, Pipes to)
{
	(int x, int y) = to switch
	{
		Pipes.North => (currentX, currentY - 1),
		Pipes.East => (currentX + 1, currentY),
		Pipes.South => (currentX, currentY + 1),
		_ => (currentX - 1, currentY),
	};
	Pipes direction = pipeMap[x, y] & ~GetFrom(to);
	return (x, y, direction);
}

(int x1, int y1) = (startX, startY);
(int x2, int y2) = (startX, startY);

bool[,] knownPipes = new bool[pipeMap.GetLength(0), pipeMap.GetLength(1)];
knownPipes[startX, startY] = true;

int step;
for (step = 1; ; step++)
{
	(x1, y1, direction1) = GoForward(x1, y1, direction1);
	knownPipes[x1, y1] = true;
	if ((x1, y1) == (x2, y2))
	{
		step--;
		break;
	}

	(x2, y2, direction2) = GoForward(x2, y2, direction2);
	knownPipes[x2, y2] = true;
	if ((x1, y1) == (x2, y2))
	{
		break;
	}
}

Console.WriteLine("Part 1: " + step);

//Determine the inside area of the pipe map by scanning through it
bool isInsideTop = false;
bool isInsideBottom = false;
int insideCount = 0;

Dictionary<Pipes, char> pipesToSymbol = new() { { Pipes.North | Pipes.South, '\u2502' },
	{ Pipes.East | Pipes.West, '\u2500' }, {Pipes.North | Pipes.East, '\u2514' },
	{ Pipes.North | Pipes.West, '\u2518' }, { Pipes.South | Pipes.West, '\u2510' },
	{ Pipes.East | Pipes.South, '\u250C' } };

ConsoleColor defaultColor = Console.ForegroundColor;
for (int y = 0; y < pipeMap.GetLength(1); y++)
{
	for (int x = 0; x < pipeMap.GetLength(0); x++)
	{
		//Determine inside or outside
		if (knownPipes[x, y])
		{
			if ((pipeMap[x, y] & Pipes.North) > 0)
			{
				isInsideTop = !isInsideTop;
			}
			if ((pipeMap[x, y] & Pipes.South) > 0)
			{
				isInsideBottom = !isInsideBottom;
			}
		}
		else if (isInsideTop && isInsideBottom)
		{
			insideCount++;
		}

		//Draw the map
		if ((x == startX) && (y == startY))
		{
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write("@");
		}
		else if (knownPipes[x, y])
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(pipesToSymbol[pipeMap[x, y]]);
		}
		else if (isInsideTop && isInsideBottom)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("*");
		}
		else
		{
			Console.Write(" ");
		}
	}
	Console.WriteLine();
}
Console.ForegroundColor = defaultColor;

Console.WriteLine();
Console.WriteLine("Part 2: " + insideCount);

[Flags] enum Pipes { None = 0, North = 0b0001, East = 0b0010, South = 0b0100, West = 0b1000 };