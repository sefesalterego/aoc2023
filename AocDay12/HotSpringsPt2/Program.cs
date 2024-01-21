// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using System.Text;

Console.WriteLine("Hello, World!");

const char OK = '.';
const char BROKEN = '#';
const char UNKNOWN = '?';


Fragment? ReadLine(string pattern, int[] groupSizes)
{
	StringBuilder workingPattern = new(pattern);

	int maxGroupSize = 0;
	int maxIndex = -1;
	for (int index = 0; index < groupSizes.Length; index++)
	{
		if (groupSizes[index] > maxGroupSize)
		{
			maxGroupSize = groupSizes[index];
			maxIndex = index;
		}
	}

	int[] leftGroupSizes = groupSizes.Take(maxIndex).ToArray();
	int completedGroupCount = 0;
	int leftPatternIndex = 0;
	for (int leftGroupIndex = 0; leftGroupIndex < leftGroupSizes.Length; leftGroupIndex++)
	{
		while (workingPattern[leftPatternIndex] == OK)
		{
			leftPatternIndex++;

			if (leftPatternIndex >= workingPattern.Length)
			{
				return null;
			}
		}
		while (workingPattern[leftPatternIndex] == BROKEN)
		{
			if (leftGroupSizes[leftGroupIndex] == 0)
			{
				return null;
			}
			leftGroupSizes[leftGroupIndex]--;
			leftPatternIndex++;

			if (leftGroupSizes[leftGroupIndex] == 0)
			{
				completedGroupCount++;
			}

			if (leftPatternIndex >= workingPattern.Length)
			{
				return null;
			}

			if ((leftGroupSizes[leftGroupIndex] > 0) && (workingPattern[leftPatternIndex] == UNKNOWN))
			{
				workingPattern[leftPatternIndex] = BROKEN;
			}
		}
		if (workingPattern[leftPatternIndex] == UNKNOWN)
		{
			if (leftGroupSizes[leftGroupIndex] == 0)
			{
				workingPattern.Remove(leftPatternIndex, 1);
				workingPattern.Insert(leftPatternIndex, OK);
			}
			else
			{
				break;
			}
		}
	}

	int mainGroupStartIndex = leftPatternIndex + leftGroupSizes.Skip(completedGroupCount).Sum(val => val + 1);
	while (workingPattern[mainGroupStartIndex] == OK)
	{
		mainGroupStartIndex++;
		if (mainGroupStartIndex >= workingPattern.Length)
		{
			return null;
		}
	}
}

class Fragment
{
	public Fragment? Left { get; }
	public Fragment? Right { get; }
	public string Pattern { get; }
	public int[] GroupSizes { get; }

	public Fragment(string line)
	{

	}
}