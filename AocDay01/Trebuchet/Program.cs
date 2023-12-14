using System;
using System.IO;
using System.Linq;

Console.WriteLine(
	File.ReadAllLines("Input.txt")
		.Where(line => line.Any(chr => Char.IsDigit(chr)))
		.Sum(line =>
			Int32.Parse($"{line.First(chr => Char.IsDigit(chr))}{line.Last(chr => Char.IsDigit(chr))}")));
