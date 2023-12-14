using System;
using System.IO;
using System.Linq;
using System.Text;

char? GetDigit(string raw, int index)
{
	Dictionary<string, char> map = new(StringComparer.InvariantCultureIgnoreCase) {
		{ "one", '1' },
		{ "two", '2' },
		{ "three", '3' },
		{ "four", '4' },
		{ "five", '5' },
		{ "six", '6' },
		{ "seven", '7' },
		{ "eight", '8' },
		{ "nine", '9' },
	};

	if (Char.IsDigit(raw[index]))
	{
		return raw[index];
	}
	foreach ((string text, char num) in map)
	{
		if (raw[index..].StartsWith(text))
		{
			return num;
		}
	}
	return null;
}

char Get(string raw, bool last = false)
{
	for (int i = 0; i < raw.Length; i++)
	{
		char? digit = GetDigit(raw, last ? raw.Length - i - 1 : i);
		if (digit is not null)
		{
			return digit.Value;
		}
	}
	throw new InvalidOperationException();
}

Console.WriteLine(
	File.ReadAllLines("Input.txt")
		.Where(line => line.Length > 0)
		.Sum(line =>
			Int32.Parse($"{Get(line)}{Get(line, last: true)}")));
