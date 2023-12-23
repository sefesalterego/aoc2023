
using System.Xml.Serialization;

int GetReflection(bool[,] pattern, Reflection reflection)
{
	//Get the the transpose factors and dimension length for the reflection check
	int transposeX = reflection == Reflection.Row ? 0 : 1;
	int transposeY = 1 - transposeX;

	int xLenght = 1 - transposeX;
	int yLength = 1 - transposeY;

	bool DoesReflect(int x, int y, int index)
	{
		int leftIndex = x - index - 1;
		int rightIndex = x + index;

		bool left = pattern[leftIndex * transposeX + y * transposeY, y * transposeX + leftIndex * transposeY];
		bool right = pattern[rightIndex * transposeX + y * transposeY, y * transposeX + rightIndex * transposeY];

		return left == right;
	}

	//Check reflection in first row
	int lineOfReflection = 0;
	for (int x = 1; x < pattern.GetLength(xLenght); x++)
	{
		int leftCount = x;
		int rightCount = pattern.GetLength(xLenght) - x;
		int count = Math.Min(leftCount, rightCount);

		int reflectCount = 0;
		for (int index = 0; index < count; index++)
		{
			bool isReflecting = true;
			for (int y = 0; y < pattern.GetLength(yLength); y++)
			{
				if (!DoesReflect(x, y, index))
				{
					isReflecting = false;
					break;
				}
			}

			if (isReflecting)
			{
				reflectCount++;
			}
			else
			{
				break;
			}
		}

		if ((x - reflectCount == 0) || (x + reflectCount == pattern.GetLength(xLenght)))
		{
			lineOfReflection = x;
		}
	}

	return lineOfReflection;
}

string[] input = File.ReadAllLines("Input.txt");

List<bool[,]> patterns = [];
List<string> currentContent = [];

void AddPattern()
{
	if (currentContent.Count > 0)
	{
		bool[,] pattern = new bool[currentContent[0].Length, currentContent.Count];
		for (int y = 0; y < currentContent.Count; y++)
		{
			for (int x = 0; x < currentContent[y].Length; x++)
			{
				pattern[x, y] = currentContent[y][x] == '#';
			}
		}
		patterns.Add(pattern);
	}
}

foreach (string line in input)
{
	if (String.IsNullOrWhiteSpace(line))
	{
		AddPattern();
		if (currentContent.Count > 0)
		{
			currentContent.Clear();
		}
		continue;
	}
	currentContent.Add(line);
}
AddPattern();

//Part 1
Console.WriteLine("Part 1: " +
	(patterns.Sum(p => GetReflection(p, Reflection.Column)) +
	100 * patterns.Sum(p => GetReflection(p, Reflection.Row))));

//Part 2:
int GetWithSmudge(bool[,] pattern)
{
	//Functions to compare columns and rows
	bool TryCorrectColumns(int x)
	{
		int count = Math.Min(x, pattern.GetLength(0) - x);

		bool hasSmudge = false;
		for (int index = 0; index < count; index++)
		{
			int differenceCount = 0;
			int lastDifferenceX = 0;
			int lastDifferenceY = 0;
			for (int y = 0; y < pattern.GetLength(1); y++)
			{
				if (pattern[x - index - 1, y] != pattern[x + index, y])
				{
					differenceCount++;
					lastDifferenceX = x + index;
					lastDifferenceY = y;
				}
				if (differenceCount > 1)
				{
					break;
				}
			}
			if (differenceCount > 1)
			{
				return false;
			}
			else if (differenceCount == 1)
			{
				hasSmudge = true;
			}
		}
		return hasSmudge;
	}

	bool TryCorrectRows(int y)
	{
		int count = Math.Min(y, pattern.GetLength(1) - y);

		bool hasSmudge = false;
		for (int index = 0; index < count; index++)
		{
			int differenceCount = 0;
			int lastDifferenceX = 0;
			int lastDifferenceY = 0;
			for (int x = 0; x < pattern.GetLength(0); x++)
			{
				if (pattern[x, y - index - 1] != pattern[x, y + index])
				{
					differenceCount++;
					lastDifferenceX = x;
					lastDifferenceY = y + index;
				}
				if (differenceCount > 1)
				{
					break;
				}
			}
			if (differenceCount > 1)
			{
				return false;
			}
			else if (differenceCount == 1)
			{
				hasSmudge = true;
			}
		}
		return hasSmudge;
	}

	//Look for a smudge in the columns from the left
	int middleX = pattern.GetLength(0) / 2;
	for (int x = 1; x <= middleX; x++)
	{
		if (TryCorrectColumns(x))
		{
			return x;
		}
	}

	//Look for a smudge in the columns from the right
	for (int x = pattern.GetLength(0) - 1; x > middleX; x--)
	{
		if (TryCorrectColumns(x))
		{
			return x;
		}
	}

	//Look for a smudge in the rows from the top
	int middleY = pattern.GetLength(1) / 2;
	for (int y = 1; y <= middleY; y++)
	{
		if (TryCorrectRows(y))
		{
			return y * 100;
		}
	}

	//Look for a smudge in the rows from the bottom
	for (int y = pattern.GetLength(1) - 1; y > middleY; y--)
	{
		if (TryCorrectRows(y))
		{
			return y * 100;
		}
	}

	throw new InvalidOperationException();
}

Console.WriteLine("Part 2: " + patterns.Sum(GetWithSmudge));

enum Reflection { Row, Column }