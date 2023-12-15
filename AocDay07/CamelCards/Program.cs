static int GetResult(bool withJoker)
{
	List<Hand> hands = File.ReadAllLines("Input.txt").Select(line => Hand.Parse(line, withJoker)).ToList();
	hands.Sort();

	int result = 0;
	for (int index = 0; index < hands.Count; index++)
	{
		result += (index + 1) * hands[index].Bid;
	}

	return result;
}

Console.WriteLine("Part 1: " + GetResult(withJoker: false));
Console.WriteLine("Part 2: " + GetResult(withJoker: true));

enum Value { High, Pair, TwoPair, Three, FullHouse, Four, Five }

struct Hand(string cards, int bid, bool withJoker) : IComparable<Hand>
{
	private static readonly Dictionary<char, char> s_charMap = new() { { 'A', 'M' }, { 'K', 'L' }, { 'Q', 'K' },
		{ 'J', 'J' }, { 'T', 'I' }, { '9', 'H' }, { '8', 'G' }, { '7', 'F' }, { '6', 'E' }, { '5', 'D' }, { '4', 'C' },
		{ '3', 'B' }, { '2', 'A' }
	};

	private static readonly Dictionary<char, char> s_jokerMap = new() { { 'A', 'M' }, { 'K', 'L' }, { 'Q', 'K' },
		{ 'J', '0' }, { 'T', 'I' }, { '9', 'H' }, { '8', 'G' }, { '7', 'F' }, { '6', 'E' }, { '5', 'D' }, { '4', 'C' },
		{ '3', 'B' }, { '2', 'A' }
	};

	public string Cards { get; } = withJoker ?
		new(cards.Select(chr => s_jokerMap[chr]).ToArray()) :
		new(cards.Select(chr => s_charMap[chr]).ToArray());

	public int Bid { get; } = bid;
	public Value Value { get; } = withJoker ? GetValueWithJoker(cards) : GetValue(cards);

	public int CompareTo(Hand other) => Value != other.Value ?
		Value.CompareTo(other.Value) :
		Cards.CompareTo(other.Cards);

	private static Value GetValue(string cards)
	{
		int[] counts = cards.Distinct().Select(chr => cards.Count(chr2 => chr2 == chr)).ToArray();
		return
			counts.Contains(5) ?
				Value.Five :
			counts.Contains(4) ?
				Value.Four :
			counts.Contains(3) && counts.Contains(2) ?
				Value.FullHouse :
			counts.Contains(3) ?
				Value.Three :
			counts.Count(count => count == 2) > 1 ?
				Value.TwoPair :
			counts.Contains(2) ?
				Value.Pair :
				Value.High;
	}

	private static Value GetValueWithJoker(string cards)
	{
		Value baseValue = GetValue(cards.Replace("J", ""));
		int jokerCount = cards.Count(chr2 => chr2 == 'J');
		return jokerCount switch
		{
			0 => baseValue,
			1 => baseValue switch
			{
				Value.High => Value.Pair,
				Value.Pair => Value.Three,
				Value.TwoPair => Value.FullHouse,
				Value.Three => Value.Four,
				_ => Value.Five,
			},
			2 => baseValue switch
			{
				Value.High => Value.Three,
				Value.Three => Value.Five,
				_ => Value.Four,
			},
			3 => baseValue switch
			{
				Value.High => Value.Four,
				_ => Value.Five,
			},
			_ => Value.Five,
		};

	}

	public static Hand Parse(string line, bool withJoker)
	{
		string[] parts = line.Split();
		return new(parts[0], Int32.Parse(parts[1]), withJoker);
	}
}